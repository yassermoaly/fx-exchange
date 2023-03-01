using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;
using Models.DTOs.Rate;
using Moq;
using ServiceLayer;
using ServiceLayer.Interfaces;

namespace UnitTesting
{
    [TestClass]
    public class FxTransactionUnitTest
    {
        Helper _helper;
        public FxTransactionUnitTest()
        {
            _helper=new Helper();
        }
        [TestMethod]
        public async Task ExpiredConverstionRate()
        {
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, 300);

            bool IsExpired = true;

            ISecurityService SecurityService = Mock.Of<ISecurityService>(l => l.LoadSecureObj<DTOConversionRateResponse>(It.IsAny<string>(),out IsExpired) == new DTOConversionRateResponse());

            var FxTranasctionService = new FxTransactionService(_helper.FxTransactionRepository, _helper.CurrencyService, _helper.AccountService, SecurityService, _helper.PresistanceService, _helper.RateLimitService, _helper.FxTransactionDetailTypeService,_helper.HolderService, _helper.Mapper);

            var AppException = await Assert.ThrowsExceptionAsync<ApplicationException>(async() => await FxTranasctionService.Create(new DTOCreateFxTransactionRequest()
            {
                ConversionRateRequestId = ConverstionRate.ConversionRateRequestId
            }));

            Assert.IsTrue(AppException.Message.Contains("Conversion rate is expired"));
        }
        [TestMethod]
        public async Task BurnedConversionRate()
        {
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, 300);

            var PresistanceGetTask = Task.Run(() =>
            {
                return default(Guid?);
            });

            IPresistanceService PresistanceService = Mock.Of<IPresistanceService>(l => l.Get<Guid?>(It.IsAny<string>()) == PresistanceGetTask);

            var FxTranasctionService = new FxTransactionService(_helper.FxTransactionRepository, _helper.CurrencyService, _helper.AccountService, _helper.SecurityService, PresistanceService, _helper.RateLimitService, _helper.FxTransactionDetailTypeService, _helper.HolderService, _helper.Mapper);

            var Exception = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await FxTranasctionService.Create(new DTOCreateFxTransactionRequest()
            {
                ConversionRateRequestId = ConverstionRate.ConversionRateRequestId
            }));

            Assert.IsTrue(Exception.Message.Contains("Conversion rate is no longer available"));
        }

        [TestMethod]
        public async Task InSufficientBalance()
        {
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, 1000);
            Guid? ReferenceId = ConverstionRate?.ReferenceId;


            Mock<IPresistanceService> MockPresistanceService = new Mock<IPresistanceService>();
            MockPresistanceService.Setup(l => l.Increment(It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.Run(()=>(long)5));
            MockPresistanceService.Setup(l => l.Get<Guid?>(It.IsAny<string>())).Returns(Task.Run(()=> ReferenceId));
            var RateLimitService = new RateLimitService(MockPresistanceService.Object, _helper.Configuration);


            
            IHolderRepository HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(()=>_helper.Holder));
            var HolderService = new HolderService(HolderRepository, _helper.Mapper);

    
            var MockAccountRepository = new Mock<IAccountRepository>();
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), _helper.CurrencyUSD.Id)).Returns(Task.Run(()=> new Account() { Id = 1,Balance = 200,CurrencyId = _helper.CurrencyUSD.Id,HolderId = _helper.Holder.Id} ?? default));
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), _helper.CurrencyEUR.Id)).Returns(Task.Run(() => new Account() { Id = 2,Balance = 200,CurrencyId = _helper.CurrencyEUR.Id,HolderId = _helper.Holder.Id} ?? default));


            var AccountService = new AccountService(MockAccountRepository.Object, _helper.Mapper);


            var FxTranasctionService = new FxTransactionService(_helper.FxTransactionRepository, _helper.CurrencyService, AccountService, _helper.SecurityService, MockPresistanceService.Object, RateLimitService, _helper.FxTransactionDetailTypeService, HolderService, _helper.Mapper);

            var Exception = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await FxTranasctionService.Create(new DTOCreateFxTransactionRequest()
            {
                ConversionRateRequestId = ConverstionRate?.ConversionRateRequestId??string.Empty
            }));

            Assert.IsTrue(Exception.Message.Contains("Insufficient Balance in account"));
        }


        [TestMethod]
        public async Task ExceedLimitPerHour()
        {
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, 50);
            Guid? ReferenceId = ConverstionRate?.ReferenceId;

            Mock<IPresistanceService> MockPresistanceService = new Mock<IPresistanceService>();
            MockPresistanceService.Setup(l => l.Increment(It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.Run(() => (long)11));
            MockPresistanceService.Setup(l => l.Get<Guid?>(It.IsAny<string>())).Returns(Task.Run(() => ReferenceId));
            
            var RateLimitService = new RateLimitService(MockPresistanceService.Object, _helper.Configuration);


            var HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(() => _helper.Holder ?? default));
            var HolderService = new HolderService(HolderRepository, _helper.Mapper);

            var MockAccountRepository = new Mock<IAccountRepository>();
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), _helper.CurrencyUSD.Id)).Returns(Task.Run(() => _helper.AccountUSD ?? default));
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), _helper.CurrencyEUR.Id)).Returns(Task.Run(() => _helper.AccountEUR ?? default));


            var AccountService = new AccountService(MockAccountRepository.Object, _helper.Mapper);


            var FxTranasctionService = new FxTransactionService(_helper.FxTransactionRepository, _helper.CurrencyService, AccountService, _helper.SecurityService, MockPresistanceService.Object, RateLimitService, _helper.FxTransactionDetailTypeService, _helper.HolderService, _helper.Mapper);

            var Exception = await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await FxTranasctionService.Create(new DTOCreateFxTransactionRequest()
            {
                ConversionRateRequestId = ConverstionRate?.ConversionRateRequestId ?? string.Empty
            }));

            Assert.IsTrue(Exception.Message.Contains("Exceeds limits per hour"));
        }

        

        [TestMethod]
        public async Task CreateTrade_BuyUSD_SellEUR_FixedSideSell()
        {
            double AccountUSDBalance = 1000;
            double AccountEURBalance = 1000;

            double SellAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Sell, SellAmount);

            var CreateResult = await _helper.CreateTrade(ConverstionRate, AccountUSDBalance, AccountEURBalance);


            Assert.AreEqual(CreateResult.Status,GResponseStatus.Success);

            var BuyTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Buy"));
            var SellTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Sell"));

            Assert.AreEqual(BuyTransactionDetails?.AccountBalancePost, AccountUSDBalance  + SellAmount*_helper.EURTOUSD);
            Assert.AreEqual(SellTransactionDetails?.AccountBalancePost, AccountEURBalance -  SellAmount);
        }


        [TestMethod]
        public async Task CreateTrade_BuyUSD_SellEUR_FixedSideBuy()
        {
            double AccountUSDBalance = 1000;
            double AccountEURBalance = 1000;

            double BuyAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyUSD.ISOCode, _helper.CurrencyEUR.ISOCode, FxTransactionFixedSideEnum.Buy, BuyAmount);

            var CreateResult = await _helper.CreateTrade(ConverstionRate, AccountUSDBalance, AccountEURBalance);


            Assert.AreEqual(CreateResult.Status, GResponseStatus.Success);

            var BuyTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Buy"));
            var SellTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Sell"));

            Assert.AreEqual(BuyTransactionDetails?.AccountBalancePost, AccountUSDBalance + BuyAmount);
            Assert.AreEqual(SellTransactionDetails?.AccountBalancePost, AccountEURBalance - (BuyAmount / _helper.EURTOUSD));
        }


        [TestMethod]
        public async Task CreateTrade_BuyEUR_SellUSD_FixedSideSell()
        {
            double AccountUSDBalance = 1000;
            double AccountEURBalance = 1000;

            double SellAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyEUR.ISOCode, _helper.CurrencyUSD.ISOCode, FxTransactionFixedSideEnum.Sell, SellAmount);

            var CreateResult = await _helper.CreateTrade(ConverstionRate, AccountUSDBalance, AccountEURBalance);


            Assert.AreEqual(CreateResult.Status, GResponseStatus.Success);

            var BuyTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Buy"));
            var SellTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Sell"));

            Assert.AreEqual(BuyTransactionDetails?.AccountBalancePost, AccountUSDBalance + SellAmount * _helper.USDTOEUR);
            Assert.AreEqual(SellTransactionDetails?.AccountBalancePost, AccountEURBalance - SellAmount);
        }


        [TestMethod]
        public async Task CreateTrade_BuyEUR_SellUSD_FixedSideBuy()
        {
            double AccountUSDBalance = 1000;
            double AccountEURBalance = 1000;

            double BuyAmount = 100;
            var ConverstionRate = await _helper.CreateConverstionRate(_helper.CurrencyEUR.ISOCode, _helper.CurrencyUSD.ISOCode, FxTransactionFixedSideEnum.Buy, BuyAmount);

            var CreateResult = await _helper.CreateTrade(ConverstionRate, AccountUSDBalance, AccountEURBalance);


            Assert.AreEqual(CreateResult.Status, GResponseStatus.Success);

            var BuyTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Buy"));
            var SellTransactionDetails = CreateResult?.Data?.FxTransactionDetails.First(r => r.FxTransactionDetailType.Name.Equals("Sell"));

            Assert.AreEqual(BuyTransactionDetails?.AccountBalancePost, AccountUSDBalance + BuyAmount);
            Assert.AreEqual(SellTransactionDetails?.AccountBalancePost, AccountEURBalance - (BuyAmount / _helper.USDTOEUR));
        }


        [TestMethod]
        public async Task Filter()
        {
            var FilterRequest = new FilterRequest<DTOFxTransactionFilter>()
            {
                PageLength = 10,
                Start = 1,
                SearchData = new DTOFxTransactionFilter()
                {
                    DateFrom = DateTime.Now.AddYears(-1),
                    DateTo = DateTime.Now.AddDays(1)
                }
            };
            double FxRate = 1.2;
            IFxTransactionRepository MockFxTransactionRepository = Mock.Of<IFxTransactionRepository>(l => l.Filter(FilterRequest) == Task.Run(() => new FilterResponse<FxTransaction>()
            {
                TotalCount = 1,
                Data = new List<FxTransaction>(new FxTransaction[] { 
                    new FxTransaction()
                    {
                        Id = 1,
                        HolderId = _helper.Holder.Id,
                        Holder = _helper.Holder,
                        FxRate = FxRate,
                        FxTransactionDetails = new List<FxTransactionDetail>(new FxTransactionDetail[]
                        {
                            new FxTransactionDetail(){ 
                                Account = new Account()
                                {
                                    Id = 1,
                                    Balance = 100,
                                    Currency = _helper.CurrencyUSD,
                                    CurrencyId = _helper.CurrencyUSD.Id,                                    
                                }
                            }
                        }),
                        CreatedOn = DateTime.Now
                    }
                })
            }));

            var FxTransactionService = new FxTransactionService(MockFxTransactionRepository,_helper.CurrencyService,_helper.AccountService,_helper.SecurityService,_helper.PresistanceService,_helper.RateLimitService,_helper.FxTransactionDetailTypeService,_helper.HolderService, _helper.Mapper);
            var result = await FxTransactionService.Filter(FilterRequest);
            Assert.AreEqual(result.Data.Count, 1);
        }

    }
}