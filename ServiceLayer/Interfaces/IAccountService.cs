using Models.Data;
using Models.DTOs.Account;

namespace ServiceLayer.Interfaces
{
    public interface IAccountService
    {
        Task<List<DTOAccount>> GetHolderBalances(long HolderId);
        Task<Account> GetByHolderAndCurrency(long HolderId, Currency Currency);
        void BuyAmount(Account Account, double Amount);
        void SellAmount(Account Account, double Amount);
    }
}
