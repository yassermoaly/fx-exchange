using Integration.Interfaces;
using Integration.Models.Fixer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.DTOs.General;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Integration
{
    public class FixerFxExchangeRateWebService : IFxExchangeRateWebService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IFxExchangeRateWebService> _logger;
        private readonly IConfiguration _configuration;
        private const string RequestBluePrint = "{Service} {Type} {Uri} {CorrelationId}";
        private const string ResponseBluePrint = "{Service} {Type} {Uri} {StatusCode} {Body} {CorrelationId} {TimeElapsed}";
        public FixerFxExchangeRateWebService(IHttpClientFactory httpClientFactory, ILogger<IFxExchangeRateWebService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<double> GetRate(string From, string To)
        {
            Guid CorrelationId = Guid.NewGuid();
            var stopwatch = new Stopwatch();

            string RequestUrl = $"{_configuration["Fixer:BaseUrl"]}/convert?to={To}&from={From}&amount={1}";
            var client = _httpClientFactory.CreateClient("Fixer");
            _logger.LogInformation(RequestBluePrint, "FixerFxExchange", "Response", RequestUrl, CorrelationId);

            HttpRequestMessage RequestMessage = new(HttpMethod.Get, RequestUrl);
                

            RequestMessage.Headers.Add("apikey", _configuration["Fixer:APIKey"]);
            stopwatch.Start();
            var ResponseMessage = await client.SendAsync(RequestMessage);
            stopwatch.Stop();
            

            string Body = await ResponseMessage.Content.ReadAsStringAsync();
            _logger.LogInformation(ResponseBluePrint, "FixerFxExchange", "Response",RequestUrl, (int)ResponseMessage.StatusCode, Body, CorrelationId, stopwatch.ElapsedMilliseconds);
         

            if (ResponseMessage.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(Body))
            {
                var Response = JsonConvert.DeserializeObject<FixerResponse>(Body);
                if (Response != null && Response.Success.HasValue && Response.Success.Value && Response.Result.HasValue)
                {
                    return Response.Result.Value;
                }
            }
            _logger.LogError(ResponseBluePrint, "FixerFxExchange", "Response", RequestUrl, (int)ResponseMessage.StatusCode, Body, CorrelationId, stopwatch.ElapsedMilliseconds);
            throw new Exception($"UnExpected Error while calling Fixer End Point, StatusCode : {(int)ResponseMessage.StatusCode}, ResponseBody : {ResponseMessage}");
        }
    }
}
