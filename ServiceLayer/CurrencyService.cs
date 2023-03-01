using DataAccessLayer.Interfaces;
using Models.Data;
using ServiceLayer.Interfaces;

namespace ServiceLayer
{
    public class CurrencyService: ICurrencyService
    {
        private static ICollection<Currency> _currencies = new HashSet<Currency>();

        public Currency GetByISO(string ISO)
        {
            return _currencies.FirstOrDefault(r=>r.ISOCode == ISO) ??  throw new ApplicationException($"UnSupported Currency {ISO}");
        }
        public async Task Load(ICurrencyRepository _currencyRepository)
        {           
            _currencies = await _currencyRepository.GetAllAsync();
        }       
    }
}
