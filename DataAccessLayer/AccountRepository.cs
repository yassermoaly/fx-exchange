using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Data;

namespace DataAccessLayer
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(FxExchangeDBContext context) : base(context)
        {
        }

        public async Task<List<Account>> GetHolderBalances(long HolderId)
        {
            return await Where(r => r.HolderId == HolderId).Include(r=>r.Currency).ToListAsync();
        }
        public async Task<Account?> GetByHolderAndCurrency(long HolderId,short CurrencyId)
        {
            return await FirstOrDefaultAsync(r => r.HolderId == HolderId && r.CurrencyId == CurrencyId);
        }
    }
}
