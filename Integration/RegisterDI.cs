using Integration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration
{
    public static class RegisterDI
    {
        public static void RegisterIntegrationLayerDI(this IServiceCollection ServiceCollection, IConfiguration configuration)
        {
            ServiceCollection.AddSingleton<IFxExchangeRateWebService, FixerFxExchangeRateWebService>();
        }
    }
}
