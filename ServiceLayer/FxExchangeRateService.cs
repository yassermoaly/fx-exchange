using Integration.Interfaces;
using Microsoft.Extensions.Configuration;
using Models.Data;
using Models.DTOs.Rate;
using ServiceLayer.Interfaces;

namespace ServiceLayer
{
    public class FxExchangeRateService : IFxExchangeRateService
    {
        private readonly IFxExchangeRateWebService _fxExchangeRate;
        private readonly ICurrencyService _currencyService;
        private readonly IPresistanceService _presistanceService;
        private readonly ISecurityService _securityService;
        private readonly IHolderService _holderService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public FxExchangeRateService(IFxExchangeRateWebService fxExchangeRate, ICurrencyService currencyService, IPresistanceService presistanceService, ISecurityService securityService, IHolderService holderService,IAccountService accountService, IConfiguration configuration)
        {
            _fxExchangeRate = fxExchangeRate;
            _presistanceService = presistanceService;
            _securityService = securityService;
            _currencyService = currencyService;
            _holderService = holderService; 
            _accountService = accountService;
            _configuration = configuration;
        }
        private async Task<double> GetRate(string SellCurrency, string BuyCurrency)
        {
            double? Rate = 0;
            string FxKey = $"FX-{SellCurrency}-{BuyCurrency}";
            if (!await _presistanceService.KeyExists(FxKey))
            {
                string LockKey = $"LK-{FxKey}";
                try
                {
                    await _presistanceService.AcquireLock(LockKey, TimeSpan.FromMinutes(1), 10000, async () => await _presistanceService.KeyExists(FxKey));
                    if (!await _presistanceService.KeyExists(FxKey))
                    {
                        Rate = await _fxExchangeRate.GetRate(SellCurrency, BuyCurrency);
                        await _presistanceService.Set(FxKey, Rate, TimeSpan.FromMinutes(double.Parse(_configuration["CurrencyCacheInMinutes"]??"1")));
                    }
                    else
                        Rate = await _presistanceService.Get<double>(FxKey);
                }
                finally
                {
                    await _presistanceService.ReleaseLock(LockKey);
                }
            }
            else
                Rate = await _presistanceService.Get<double>(FxKey);

            return Rate.Value;
        }
        public async Task<DTOConversionRateResponse> GetOffer(DTOConversionRateRequest Posted)
        {
            var SellCurrency = _currencyService.GetByISO(Posted.SellCurrency);
            var BuyCurrency = _currencyService.GetByISO(Posted.BuyCurrency);

            var Holder = await _holderService.Get(Posted.HolderId);

            var SellAccount = await _accountService.GetByHolderAndCurrency(Holder.Id, SellCurrency);
            var BuyAccount = await _accountService.GetByHolderAndCurrency(Holder.Id, BuyCurrency);

            double ConversionRate = await GetRate(SellCurrency.ISOCode, BuyCurrency.ISOCode);
            var Response = new DTOConversionRateResponse()
            {
                ReferenceId = Guid.NewGuid(),
                BuyCurrency = BuyAccount.Currency.ISOCode,
                SellCurrency = SellAccount.Currency.ISOCode,
                BuyAmount = Posted.FixedSide == FxTransactionFixedSideEnum.Sell ? Posted.Amount * ConversionRate : Posted.Amount,
                SellAmount = Posted.FixedSide == FxTransactionFixedSideEnum.Buy ? Posted.Amount / ConversionRate : Posted.Amount,
                ConversionRate = ConversionRate,
                HolderId = Posted.HolderId,
                Amount = Posted.Amount,
                FixedSide = Posted.FixedSide
            };

            //Holder can have at max one rate trade to finilize
            int ConversionRateValidatyInMinutes = int.Parse(_configuration["ConversionRateValidatyInMinutes"] ?? "24");
            Response.ConversionRateRequestId = _securityService.GenerateSecureObj(Response, ConversionRateValidatyInMinutes);
            await _presistanceService.Set($"holder-{Response.HolderId}-rate", Response.ReferenceId, TimeSpan.FromMinutes(ConversionRateValidatyInMinutes));
            return Response;
        }      
    }
}
