using DataAccessLayer.Interfaces;
using Integration.Interfaces;
using Models.Data;
using Models.DTOs;
using Models.DTOs.Rate;
using Moq;
using ServiceLayer;
using ServiceLayer.Interfaces;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class FxExchangeRateUnitTest
    {
        Helper _helper;
        public FxExchangeRateUnitTest()
        {
            _helper = new Helper();
        }
        [TestMethod]
        public async Task SellCurrencyNotsupported()
        {
            
            ICurrencyRepository MockCurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == Task.Run(()=>new List<Currency>(new[] { _helper.CurrencyUSD })));
            var CurrencyService = new CurrencyService();
            await CurrencyService.Load(MockCurrencyRepository);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async() =>  await _helper.RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = "USD",
                SellCurrency = "EUR",
                FixedSide = FxTransactionFixedSideEnum.Sell,
                Amount = 100,
                HolderId = 1
            }));

            Assert.AreEqual(AppException.Message, $"UnSupported Currency EUR");
        }
        [TestMethod]
        public async Task BuyCurrencyNotsupported()
        {
            ICurrencyRepository MockCurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == Task.Run(() => new List<Currency>(new[] { _helper.CurrencyEUR })));
            var CurrencyService = new CurrencyService();
            await CurrencyService.Load(MockCurrencyRepository);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await _helper.RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = "USD",
                SellCurrency = "EUR",
                FixedSide = FxTransactionFixedSideEnum.Sell,
                Amount = 100,
                HolderId = 1
            }));

            Assert.AreEqual(AppException.Message, $"UnSupported Currency USD");
        }

        [TestMethod]
        public async Task HolderDoesNotHaveAccountWithBuyCurrency()
        {
           
            IAccountRepository AccountRepository = Mock.Of<IAccountRepository>(l => l.GetByHolderAndCurrency(It.IsAny<long>(),_helper.CurrencyUSD.Id) == Task.Run(()=>_helper.AccountEUR));
            var AccountService = new AccountService(AccountRepository, _helper.Mapper);

            IHolderRepository HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(() => _helper.Holder));
            var HolderService = new HolderService(HolderRepository,_helper.Mapper);

            var RateService = new FxExchangeRateService(_helper.FxExchangeRateWebService, _helper.CurrencyService, _helper.PresistanceService, _helper.SecurityService, HolderService, AccountService,_helper.Configuration);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = "EUR",
                SellCurrency = "USD",
                FixedSide = FxTransactionFixedSideEnum.Sell,
                Amount = 100,
                HolderId = _helper.Holder.Id
            }));

            Assert.AreEqual(AppException.Message, $"Holder {_helper.Holder.Id}, doesn't have account in EUR Currency");
        }

        [TestMethod]
        public async Task HolderDoesNotHaveAccountWithSellCurrency()
        {
            IAccountRepository AccountRepository = Mock.Of<IAccountRepository>(l => l.GetByHolderAndCurrency(It.IsAny<long>(), _helper.CurrencyEUR.Id) == Task.Run(() => _helper.AccountEUR));
            var AccountService = new AccountService(AccountRepository, _helper.Mapper);
            
            IHolderRepository HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(() => _helper.Holder));
            var HolderService = new HolderService(HolderRepository, _helper.Mapper);


            var RateService = new FxExchangeRateService(_helper.FxExchangeRateWebService, _helper.CurrencyService, _helper.PresistanceService, _helper.SecurityService, HolderService, AccountService, _helper.Configuration);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = "EUR",
                SellCurrency = "USD",
                FixedSide = FxTransactionFixedSideEnum.Sell,
                Amount = 100,
                HolderId = _helper.Holder.Id
            }));

            Assert.AreEqual(AppException.Message, $"Holder {_helper.Holder.Id}, doesn't have account in USD Currency");
        }

        [TestMethod]
        public async Task HolderNotExists()
        {

            IHolderRepository HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(()=> default(Holder?)));
            var HolderService = new HolderService(HolderRepository, _helper.Mapper);
         
            var RateService = new FxExchangeRateService(_helper.FxExchangeRateWebService, _helper.CurrencyService, _helper.PresistanceService, _helper.SecurityService, HolderService, _helper.AccountService,_helper.Configuration);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = "USD",
                SellCurrency = "EUR",
                FixedSide = FxTransactionFixedSideEnum.Sell,
                Amount = 100,
                HolderId = _helper.Holder.Id
            }));

            Assert.AreEqual(AppException.Message, $"Holder Not Exists, Holder Id {_helper.Holder.Id}");
        }


       

        [TestMethod]
        public async Task GetRate_BuyUSD_SellEUR_FixedSideSell()
        {
            double SellAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Sell, SellAmount);

            Assert.AreEqual(ConverstionRate.BuyAmount, SellAmount * _helper.EURTOUSD);
            Assert.AreEqual(ConverstionRate.SellAmount, SellAmount);
        }

        [TestMethod]
        public async Task GetRate_BuyUSD_SellEUR_FixedSideBuy()
        {

            double BuyAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, BuyAmount);
            
            Assert.AreEqual(ConverstionRate.BuyAmount, BuyAmount);
            Assert.AreEqual(ConverstionRate.SellAmount, BuyAmount / _helper.EURTOUSD);
        }


        [TestMethod]
        public async Task GetRate_BuyEUR_SellUSD_FixedSideSell()
        {
            double SellAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyEUR.ISOCode, _helper.CurrencyUSD.ISOCode, FxTransactionFixedSideEnum.Sell, SellAmount);

            Assert.AreEqual(ConverstionRate.BuyAmount, SellAmount * _helper.USDTOEUR);
            Assert.AreEqual(ConverstionRate.SellAmount, SellAmount);
        }

        [TestMethod]
        public async Task GetRate_BuyEUR_SellUSD_FixedSideBuy()
        {

            double BuyAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyEUR.ISOCode, _helper.CurrencyUSD.ISOCode, FxTransactionFixedSideEnum.Buy, BuyAmount);

            Assert.AreEqual(ConverstionRate.BuyAmount, BuyAmount);
            Assert.AreEqual(ConverstionRate.SellAmount, BuyAmount / _helper.USDTOEUR);
        }
    }
}