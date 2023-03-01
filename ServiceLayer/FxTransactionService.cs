using AutoMapper;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;
using Models.DTOs.Holder;
using Models.DTOs.Rate;
using ServiceLayer.Interfaces;


namespace ServiceLayer
{
    public class FxTransactionService : IFxTransactionService
    {
        private readonly IFxTransactionRepository _fxTransactionRepository;
        private readonly IFxTransactionDetailTypeService _fxTransactionDetailTypeService;
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly ISecurityService _securityService;
        private readonly IPresistanceService _presistanceService;
        private readonly IRateLimitService _rateLimitService;
        private readonly IHolderService _holderService;
        private readonly IMapper _mapper;
        public FxTransactionService(IFxTransactionRepository fxTransactionRepository,ICurrencyService currencyService, IAccountService accountService, ISecurityService securityService, IPresistanceService presistanceService, IRateLimitService rateLimitService, IFxTransactionDetailTypeService fxTransactionDetailTypeService, IHolderService holderService, IMapper mapper)
        {
            _fxTransactionRepository = fxTransactionRepository;
            _currencyService = currencyService;
            _accountService = accountService;
            _securityService = securityService;
            _presistanceService = presistanceService;
            _fxTransactionDetailTypeService = fxTransactionDetailTypeService;           
            _rateLimitService = rateLimitService;
            _holderService = holderService;
            _mapper = mapper;
        }

        public async Task<GResponse<DTOFxTransaction>> Create(DTOCreateFxTransactionRequest Posted)
        {
            if(string.IsNullOrEmpty(Posted.ConversionRateRequestId))
                throw new ApplicationException($"ConversionRateRequestId is required");

            var ConversationRate = _securityService.LoadSecureObj<DTOConversionRateResponse>(Posted.ConversionRateRequestId, out bool IsExpired);
            if (IsExpired || ConversationRate == null)
                throw new ApplicationException($"Conversion rate is expired, ConversionRateRequestId=>{Posted.ConversionRateRequestId}");

            Guid? RateReferenceNumber = await _presistanceService.Get<Guid?>($"holder-{ConversationRate.HolderId}-rate");
            if(ConversationRate.ReferenceId.CompareTo(RateReferenceNumber) != 0)
                throw new ApplicationException($"Conversion rate is no longer available, ConversionRateRequestId=>{Posted.ConversionRateRequestId}");

            

            var BuyCurreny =  _currencyService.GetByISO(ConversationRate.BuyCurrency);
            var SellCurreny =  _currencyService.GetByISO(ConversationRate.SellCurrency);

            var BuyAccount = await _accountService.GetByHolderAndCurrency(ConversationRate.HolderId, BuyCurreny);
            var SellAccount = await _accountService.GetByHolderAndCurrency(ConversationRate.HolderId, SellCurreny);

            double BuyAccountPreBalance = BuyAccount.Balance;
            double SellAccountPreBalance = SellAccount.Balance;

            _accountService.SellAmount(SellAccount, ConversationRate.SellAmount);

            _accountService.BuyAmount(BuyAccount, ConversationRate.BuyAmount);

            await _rateLimitService.NotExceedLimits(ConversationRate.HolderId);

            var BuyFxTransactionDetail = new FxTransactionDetail()
            {
                FxTransactionDetailTypeId = FxTransactionDetailTypeValues.Buy,
                Amount = ConversationRate.BuyAmount,
                AccountId = BuyAccount.Id,
                AccountBalancePre = BuyAccountPreBalance,
                AccountBalancePost = BuyAccount.Balance
            };
            var SellFxTransactionDetail = new FxTransactionDetail()
            {
                FxTransactionDetailTypeId = FxTransactionDetailTypeValues.Sell,
                Amount = ConversationRate.SellAmount,
                AccountId = SellAccount.Id,
                AccountBalancePre = SellAccountPreBalance,
                AccountBalancePost = SellAccount.Balance
            };

            var FxTransaction = new FxTransaction()
            {
                HolderId = ConversationRate.HolderId,
                Amount = ConversationRate.Amount,
                FxTransactionFixedSideId = (short)ConversationRate.FixedSide,
                FxRate = ConversationRate.ConversionRate,
                FxTransactionDetails = new List<FxTransactionDetail>(new FxTransactionDetail[]
                {
                    BuyFxTransactionDetail,
                    SellFxTransactionDetail
                }),
                CreatedOn = DateTime.Now
            };
            await _fxTransactionRepository.AddAsync(FxTransaction);
                     
            await _fxTransactionRepository.CommitAsync();

            await _presistanceService.Remove($"holder-{ConversationRate.HolderId}-rate");


            BuyFxTransactionDetail.FxTransactionDetailType = _fxTransactionDetailTypeService.Get(FxTransactionDetailTypeValues.Buy);
            BuyFxTransactionDetail.Account = BuyAccount;


            SellFxTransactionDetail.FxTransactionDetailType = _fxTransactionDetailTypeService.Get(FxTransactionDetailTypeValues.Sell);
            SellFxTransactionDetail.Account = SellAccount;

            FxTransaction.Holder = await _holderService.Get(ConversationRate.HolderId);

            return GResponse<DTOFxTransaction>.CreateSuccess(_mapper.Map<DTOFxTransaction>(FxTransaction));
        }

        public async Task<FilterResponse<DTOFxTransaction>> Filter(FilterRequest<DTOFxTransactionFilter> Filter)
        {
            return _mapper.Map<FilterResponse<DTOFxTransaction>>(await _fxTransactionRepository.Filter(Filter));
        }
    }
}
