using APIs.MapperProfiles;
using AutoMapper;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using Integration;
using Integration.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;
using Models.DTOs.Rate;
using Moq;
using ServiceLayer;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTesting
{
    public class Helper
    {
        public IMapper Mapper { get; set; }
        public FxExchangeDBContext Context { get; set; }
        public ICurrencyRepository CurrencyRepository { get; set; }
        public IFxTransactionDetailTypeRepository FxTransactionDetailTypeRepository { get; set; }
        public IAccountRepository AccountRepository { get; set; }
        public IHolderRepository HolderRepository { get; set; }
        public FxTransactionRepository FxTransactionRepository { get; set; }


        public ICurrencyService CurrencyService { get; set; }
        public IAccountService AccountService { get; set; }
        public IFxTransactionDetailTypeService FxTransactionDetailTypeService { get; set; }

        public IHolderService HolderService { get; set; }
        public IPresistanceService PresistanceService { get; set; }
        public ISecurityService SecurityService { get; set; }
        public IFxTransactionService FxTransactionService { get; set; }

        public IFxExchangeRateWebService FxExchangeRateWebService { get;set; }


        public IFxExchangeRateService RateService { get; set; }
        public IRateLimitService RateLimitService { get; set; }        

        public IConfiguration BuildConfiguration(Dictionary<string, string?> ConfigurationValues)
        {
            return new ConfigurationBuilder()
               .AddInMemoryCollection(ConfigurationValues)
               .Build();
        }

        public Currency CurrencyUSD { get; set; }
        public Currency CurrencyEUR { get; set; }

        public List<Currency> CurrencyList;

        public Account AccountUSD { get; set; }
        public Account AccountEUR { get; set; }

        public List<Account> AccountList;

        public Holder Holder;
        public FxTransactionDetailType FxTransactionDetailTypeSell { get; set; }

        public FxTransactionDetailType FxTransactionDetailTypeBuy { get; set; }
        public List<FxTransactionDetailType> FxTransactionDetailTypeList { get; set; }
        public IConfiguration Configuration { get; set; }

        public double EURTOUSD = 1.07;
        public double USDTOEUR = 0.94;
        public Helper()
        {
            #region Init Parameters
            CurrencyUSD = new Currency()
            {
                Id = 1,
                ISOCode = "USD",
                Name = "American Dollar"
            };
            CurrencyEUR = new Currency()
            {
                Id = 2,
                ISOCode = "EUR",
                Name = "Swiss Franc"
            };
            CurrencyList = new();
            CurrencyList.Add(CurrencyUSD);
            CurrencyList.Add(CurrencyEUR);

            Holder = new Holder()
            {
                Id = 1,
                FirstName = "Yasser",
                LastName = "Moaly",
                Address = "Zayed, Egypt",
                PassportId = "A27344511"
            };

            AccountUSD = new Account() { Id = 1, CurrencyId = CurrencyUSD.Id, Balance = 1000, HolderId = Holder.Id };
            AccountEUR = new Account() { Id = 2, CurrencyId = CurrencyEUR.Id, Balance = 1000, HolderId = Holder.Id };
            AccountList = new();
            AccountList.Add(AccountUSD);
            AccountList.Add(AccountEUR);

            FxTransactionDetailTypeSell = new FxTransactionDetailType() { Id = 1, Name = "Sell" };
            FxTransactionDetailTypeBuy = new FxTransactionDetailType() { Id = 2, Name = "Buy" };
            FxTransactionDetailTypeList = new List<FxTransactionDetailType>(new FxTransactionDetailType[] { FxTransactionDetailTypeSell, FxTransactionDetailTypeBuy });
            #endregion

            var ConfigurationValues = new Dictionary<string, string?>
            {
                
                { "SecurityConfig:TokenIssuer", "fx-exchange"},
                { "SecurityConfig:TokenAudience", "fx-exchange"},
                {"SecurityConfig:SecureKey", "4031d308-b6b7-11ed-afa1-0242ac120002-4031d7cc-b6b7-11ed-afa1-0242ac120002"},
                { "FxTransactionRateLimitPerHour", "10" }
            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(ConfigurationValues)
                .Build();

            #region Init Automapper Profile
            var AutoMapperProfiles = typeof(AccountProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).Select(r => Activator.CreateInstance(r) as Profile);
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfiles(AutoMapperProfiles);
            });


            IMapper mapper = mappingConfig.CreateMapper();
            Mapper = mapper;
            #endregion

            #region Init Integration
            Mock<IFxExchangeRateWebService> MockFxExchangeRateWebService = new();
            MockFxExchangeRateWebService.Setup(r => r.GetRate(CurrencyUSD.ISOCode, CurrencyEUR.ISOCode)).Returns(Task.Run(() => USDTOEUR));
            MockFxExchangeRateWebService.Setup(r => r.GetRate(CurrencyEUR.ISOCode, CurrencyUSD.ISOCode)).Returns(Task.Run(() => EURTOUSD));

            FxExchangeRateWebService = MockFxExchangeRateWebService.Object;
            #endregion

            #region Init DB Context
            //var optionBuilder = new DbContextOptionsBuilder<FxExchangeDBContext>().UseInMemoryDatabase(databaseName: "fx-exchange-unit-test");            
            var optionBuilder = new DbContextOptionsBuilder<FxExchangeDBContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            Context = new FxExchangeDBContext(optionBuilder.Options);

            #endregion

            #region Init Repositores
            CurrencyRepository = new CurrencyRepository(Context);

            var CurrencyListTask = Task.Run(() =>
            {
                return new List<Currency>(new Currency[] { CurrencyUSD,CurrencyEUR });
            });
            CurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == CurrencyListTask);


            var ListTask = Task.Run(() =>
            {
                return new List<FxTransactionDetailType>(new FxTransactionDetailType[] { FxTransactionDetailTypeSell, FxTransactionDetailTypeBuy });
            });
            FxTransactionDetailTypeRepository = Mock.Of<IFxTransactionDetailTypeRepository>(l => l.GetAllAsync() == ListTask);


            HolderRepository = new HolderRepository(Context);
            AccountRepository= new AccountRepository(Context);
            FxTransactionRepository=new FxTransactionRepository(Context);
            #endregion

            #region Init Service

            PresistanceService = new RedisPresistanceService(Configuration);
            RateLimitService = new RateLimitService(PresistanceService, Configuration);
            SecurityService = new SecurityService(Configuration);

            CurrencyService = new CurrencyService();
            CurrencyService.Load(CurrencyRepository);

            AccountService = new AccountService(AccountRepository, Mapper);
            


            FxTransactionDetailTypeService=new FxTransactionDetailTypeService();
            FxTransactionDetailTypeService.Load(FxTransactionDetailTypeRepository);
            HolderService = new HolderService(HolderRepository, Mapper);


            FxTransactionService = new FxTransactionService(FxTransactionRepository, CurrencyService, AccountService, SecurityService, PresistanceService, RateLimitService, FxTransactionDetailTypeService,HolderService, Mapper);

            RateService = new FxExchangeRateService(FxExchangeRateWebService, CurrencyService, PresistanceService, SecurityService, HolderService, AccountService,Configuration);

            #endregion



            

        }

        public async Task<GResponse<DTOFxTransaction>> CreateTrade(DTOConversionRateResponse ConverstionRate,double AccountUSDBalance,double AccountEURBalance)
        {
            Guid? ReferenceId = ConverstionRate?.ReferenceId;

            Mock<IPresistanceService> MockPresistanceService = new();
            MockPresistanceService.Setup(l => l.Increment(It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.Run(() => (long)5));
            MockPresistanceService.Setup(l => l.Get<Guid?>(It.IsAny<string>())).Returns(Task.Run(() => ReferenceId));

            var RateLimitService = new RateLimitService(MockPresistanceService.Object, Configuration);


            var HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(() => Holder ?? default));
            var HolderService = new HolderService(HolderRepository, Mapper);

            var MockAccountRepository = new Mock<IAccountRepository>();
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), CurrencyUSD.Id)).Returns(Task.Run(() => new Account() { Id = 1, CurrencyId = CurrencyUSD.Id, Balance = AccountUSDBalance, HolderId = Holder.Id } ?? default));
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), CurrencyEUR.Id)).Returns(Task.Run(() => new Account() { Id = 1, CurrencyId = CurrencyEUR.Id, Balance = AccountEURBalance, HolderId = Holder.Id } ?? default));


            var AccountService = new AccountService(MockAccountRepository.Object, Mapper);


            var FxTranasctionService = new FxTransactionService(FxTransactionRepository, CurrencyService, AccountService, SecurityService, MockPresistanceService.Object, RateLimitService, FxTransactionDetailTypeService, HolderService, Mapper);


            return await FxTranasctionService.Create(new DTOCreateFxTransactionRequest()
            {
                ConversionRateRequestId = ConverstionRate?.ConversionRateRequestId ?? string.Empty
            });

        }
        public async Task<DTOConversionRateResponse> CreateConverstionRate(string BuyCurrency, string SellCurrency, FxTransactionFixedSideEnum ConversionRateFixedSide, double Amount)
        {
            
            IHolderRepository HolderRepository = Mock.Of<IHolderRepository>(l => l.FindAsync(It.IsAny<long>()) == Task.Run(() => Holder ?? default));

            var HolderService = new HolderService(HolderRepository, this.Mapper);
            ICurrencyRepository MockCurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == Task.Run(() => CurrencyList));
            var CurrencyService = new CurrencyService();
            await CurrencyService.Load(MockCurrencyRepository);

            var MockAccountRepository = new Mock<IAccountRepository>();
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), this.CurrencyUSD.Id)).Returns(Task.Run(() => AccountUSD??default));
            MockAccountRepository.Setup(r => r.GetByHolderAndCurrency(It.IsAny<long>(), this.CurrencyEUR.Id)).Returns(Task.Run(() => AccountEUR??default));


            var AccountService = new AccountService(MockAccountRepository.Object, this.Mapper);

            IPresistanceService PresistanceService = Mock.Of<IPresistanceService>(l => l.KeyExists(It.IsAny<string>()) == Task.Run(() => false));

            var RateService = new FxExchangeRateService(FxExchangeRateWebService, CurrencyService, PresistanceService, this.SecurityService, HolderService, AccountService, Configuration);
            return await RateService.GetOffer(new DTOConversionRateRequest()
            {
                BuyCurrency = BuyCurrency,
                SellCurrency = SellCurrency,
                FixedSide = ConversionRateFixedSide,
                Amount = Amount,
                HolderId = 1
            });
        }
    }
}
