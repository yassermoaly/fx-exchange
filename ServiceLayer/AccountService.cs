using AutoMapper;
using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs.Account;
using ServiceLayer.Interfaces;

namespace ServiceLayer
{
    public class AccountService: IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<List<DTOAccount>> GetHolderBalances(long HolderId)
        {
            return _mapper.Map<List<DTOAccount>>(await _accountRepository.GetHolderBalances(HolderId));
        }

        public async Task<Account> GetByHolderAndCurrency(long HolderId, Currency Currency)
        { 
            var Account =  (await _accountRepository.GetByHolderAndCurrency(HolderId, Currency.Id)) ?? throw new ApplicationException($"Holder {HolderId}, doesn't have account in {Currency.ISOCode} Currency");
            Account.Currency = Currency;
            return Account;
        }
        public void BuyAmount(Account Account,double Amount)
        {
            Account.Balance += Amount;
        }
        public void SellAmount(Account Account, double Amount)
        {
            Account.Balance -= Amount;
            if (Account.Balance < 0)
                throw new ApplicationException($"Insufficient Balance in account {Account.Id}");
        }
    }
}
