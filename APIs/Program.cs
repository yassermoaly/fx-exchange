using APIs.Helpers;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using Serilog.Extensions.Logging;
using Serilog;
using ServiceLayer;
using System.Text.Json.Serialization;
using APIs.Middlewares;
using ServiceLayer.Interfaces;
using DataAccessLayer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{    
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.RegisterServiceLayerDI(builder.Configuration);

builder.Services.AddHttpClient("Fixer")
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new SocketsHttpHandler
                    {
                        UseCookies = false,
                        AllowAutoRedirect = false,
                        MaxConnectionsPerServer = int.Parse(builder.Configuration["Fixer:MaxConnextionsPerServer"] ?? "2"),
                        UseProxy = false
                    };
                });
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var functionDependencyContext = DependencyContext.Load(typeof(Program).Assembly);
    var hostConfig = sp.GetRequiredService<IConfiguration>();
    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(hostConfig)
        .CreateLogger();

    return new SerilogLoggerProvider(logger, dispose: true);
});


var app = builder.Build();

#region Load Lookups
using var scope = app.Services.CreateScope();
var FxTransactionDetailTypeService = scope.ServiceProvider.GetService<IFxTransactionDetailTypeService>();
var FxTransactionDetailTypeRepository = scope.ServiceProvider.GetService<IFxTransactionDetailTypeRepository>();
if (FxTransactionDetailTypeService!=null && FxTransactionDetailTypeRepository!=null)
    await FxTransactionDetailTypeService.Load(FxTransactionDetailTypeRepository);

var CurrencyService = scope.ServiceProvider.GetService<ICurrencyService>();
var CurrencyRepository = scope.ServiceProvider.GetService<ICurrencyRepository>();
if (CurrencyService != null && CurrencyRepository != null)
    await CurrencyService.Load(CurrencyRepository);
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();
