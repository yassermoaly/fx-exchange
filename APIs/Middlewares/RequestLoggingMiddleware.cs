using Microsoft.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Models.DTOs.General;
using Models.Config;

namespace APIs.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private const string RequestBluePrint = "{Service} {Method} {Type} {Uri} {Header} {Body} {CorrelationId}";
        private const string ResponseBluePrint = "{Service} {Method} {Type} {StatusCode} {Uri} {Header} {Body} {CorrelationId} {TimeElapsed}";
        private readonly IConfiguration _configuration;
        public RequestLoggingMiddleware(RequestDelegate next, IConfiguration Configuration, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _configuration = Configuration;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Path.Value) || !context.Request.Path.Value.Contains("/api/"))
            {
                await _next(context);
                return;
            }
            Guid CorrelationId = Guid.NewGuid();
            if (_configuration["Serilog:CustomLoggingLevel"] == HttpLoggingMode.NONE)
            {
                await PerformRequest(context);
            }
            else
            {
                await LogRequest(context, CorrelationId);
                await LogResponse(context, CorrelationId);
            }
        }
        private async Task LogRequest(HttpContext context, Guid CorrelationId)
        {
            string Body = string.Empty;
            IHeaderDictionary? headers = default;
            if (_configuration["Serilog:CustomLoggingLevel"] == HttpLoggingMode.FULL)
            {
                headers = context.Response.Headers;
                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);
                Body = ReadStreamInChunks(requestStream);
            }
            _logger.LogInformation(RequestBluePrint,"Http", context.Request.Method, "Request", context.Request.Path, headers == null ? "" : headers, Body, CorrelationId);
            context.Request.Body.Position = 0;
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }
        private async Task LogResponse(HttpContext context, Guid CorrelationId)
        {
            IHeaderDictionary? headers = default;
            string resBody = string.Empty;
            Exception? ResponseException = null;
            DateTime SDate = DateTime.Now;
            if (_configuration["Serilog:CustomLoggingLevel"] == HttpLoggingMode.FULL)
            {
                var originalBodyStream = context.Response.Body;
                await using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;
                // perform user's request
                ResponseException = await PerformRequest(context);
                headers = context.Response.Headers;
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                resBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);  
            }
            else
            {
              
                ResponseException = await PerformRequest(context);
            }


            if (ResponseException == null)
                _logger.LogInformation(ResponseBluePrint, "Http", context.Request.Method, "Response", context.Response.StatusCode, context.Request.Path, headers == null ? "" : headers, resBody, CorrelationId, DateTime.Now.Subtract(SDate).TotalMilliseconds);
            else
            {
                _logger.LogError($"{ResponseBluePrint} {{Exception}}", "Http", context.Request.Method, "Response", context.Response.StatusCode, context.Request.Path, headers == null ? "" : headers, resBody, CorrelationId, DateTime.Now.Subtract(SDate).TotalMilliseconds, ResponseException);
                //await context.Response.WriteAsync(JsonConvert.SerializeObject(ResponseException), Encoding.UTF8);


            }
        }

        private async Task<Exception?> PerformRequest(HttpContext context)
        {
            Exception? ResponseException = null;
            try
            {
                await _next(context);
            }
            catch (ApplicationException ex)
            {
                ResponseException = ex;
                var AppExceptionResponse = new GResponse<object>()
                {
                    Status = GResponseStatus.Error,
                    StatusMessage = ResponseException.Message
                };

                await WriteResponseException(context, AppExceptionResponse, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {    
                ResponseException = ex;
                var ExceptionResponse = new GResponse<object>()
                {
                    Status = GResponseStatus.InternalServerError,
                    StatusMessage = bool.Parse(_configuration["IsDebuggingMode"] ?? "false")? ResponseException.Message : "Something Went Wrong",
                };
                await WriteResponseException(context, ExceptionResponse, HttpStatusCode.InternalServerError);
            }
            return ResponseException;
        }

        private static async Task WriteResponseException(HttpContext context, GResponse<object> Body, HttpStatusCode StatusCode)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var ContentBody = JsonConvert.SerializeObject(Body, serializerSettings);

            context.Response.ContentType = "application/json";
            context.Response.Headers.ContentLength = ContentBody.Length;
            context.Response.StatusCode = (int)StatusCode;
            await context.Response.WriteAsync(ContentBody, Encoding.UTF8);
        }

    }
}