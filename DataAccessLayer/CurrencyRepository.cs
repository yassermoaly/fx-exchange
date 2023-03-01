using DataAccessLayer.Interfaces;
using Models.Data;

namespace DataAccessLayer
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(FxExchangeDBContext context) : base(context)
        {
        }
        public async Task<Currency?> GetByISOCode(string ISOCode)
        {
            return await FirstOrDefaultAsync(r => r.ISOCode == ISOCode);
        }
    }
}
