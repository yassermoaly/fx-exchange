using Models.Data;

namespace DataAccessLayer.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetHolderBalances(long HolderId);
        Task<Account?> GetByHolderAndCurrency(long HolderId, short CurrencyId);
    }
}
