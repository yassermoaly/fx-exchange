using Models.Data;

namespace DataAccessLayer.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        Task<Currency?> GetByISOCode(string ISOCode);
    }
}
