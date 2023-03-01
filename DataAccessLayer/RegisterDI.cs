using DataAccessLayer;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ServiceLayer
{
    public static class RegisterDI
    {
        public static void RegisterDataAccessLayerDI(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddDbContext<FxExchangeDBContext>(options =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(configuration.GetConnectionString("DefaultConection"));
            });

            Services.AddScoped<IHolderRepository, HolderRepository>();
            Services.AddScoped<IAccountRepository, AccountRepository>();
            Services.AddScoped<IAccountRepository, AccountRepository>();
            Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            Services.AddScoped<IFxTransactionRepository, FxTransactionRepository>();
            Services.AddScoped<IFxTransactionDetailTypeRepository, FxTransactionDetailTypeRepository>();           
        }
    }
}
