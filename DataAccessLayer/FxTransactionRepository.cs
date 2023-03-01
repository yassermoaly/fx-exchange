using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;

namespace DataAccessLayer
{
    public class FxTransactionRepository : GenericRepository<FxTransaction>, IFxTransactionRepository
    {
        public FxTransactionRepository(FxExchangeDBContext context) : base(context)
        {
        }
        public async Task<FilterResponse<FxTransaction>> Filter(FilterRequest<DTOFxTransactionFilter> Filter)
        {
            var Query = GetAll();
            if (Filter.SearchData != null && Filter.SearchData.DateFrom.HasValue)
                Query = Query.Where(r => r.CreatedOn.CompareTo(Filter.SearchData.DateFrom.Value) >= 0);

            if (Filter.SearchData != null && Filter.SearchData.DateTo.HasValue)
                Query = Query.Where(r => r.CreatedOn.CompareTo(Filter.SearchData.DateTo.Value) <= 0);

            int TotalCount = await Query.CountAsync();

            return new FilterResponse<FxTransaction>()
            {
                TotalCount = TotalCount,
                Data = await Query.Include(r=>r.Holder).Include(r=>r.FxTransactionDetails).ThenInclude(c => c.Account).ThenInclude(r=>r.Currency).Include(c=>c.FxTransactionDetails).ThenInclude(c=>c.FxTransactionDetailType).OrderBy(r => r.Id).Skip(Filter.Start).Take(Filter.PageLength).ToListAsync()
            };
        }

    }
}
