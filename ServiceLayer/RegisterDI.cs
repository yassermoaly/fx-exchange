using Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class RegisterDI
    {
        public static void RegisterServiceLayerDI(this IServiceCollection Services,IConfiguration configuration)
        {
            Services.AddSingleton<IPresistanceService, RedisPresistanceService>();
            
            Services.AddScoped<IFxExchangeRateService, FxExchangeRateService>();

            Services.AddScoped<IHolderService, HolderService>();
            Services.AddScoped<IAccountService, AccountService>();
           
            Services.AddScoped<IFxTransactionService, FxTransactionService>();

            Services.AddSingleton<ICurrencyService, CurrencyService>();
            Services.AddSingleton<IFxTransactionDetailTypeService, FxTransactionDetailTypeService>();         

            Services.AddSingleton<ISecurityService, SecurityService>();

            Services.AddSingleton<IRateLimitService, RateLimitService>();

           
            Services.RegisterDataAccessLayerDI(configuration);
            Services.RegisterIntegrationLayerDI(configuration);
        }
    }
}
