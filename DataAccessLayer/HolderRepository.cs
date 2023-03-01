using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.DTOs.General;
using System.Linq;

namespace DataAccessLayer
{
    public class HolderRepository : GenericRepository<Holder>, IHolderRepository
    {
        public HolderRepository(FxExchangeDBContext context) : base(context)
        {
        }
        public async Task<FilterResponse<Holder>> Filter(FilterRequest<string> Filter)
        {
            string? SearchCriteria = Filter.SearchData;
            var Query = string.IsNullOrEmpty(SearchCriteria) ? GetAll() : Where(r => r.FirstName.Contains(SearchCriteria));
            int TotalCount = await Query.CountAsync();            

            return new FilterResponse<Holder>()
            {
                TotalCount = TotalCount,
                Data = await Query.OrderBy(r=>r.Id).Skip(Filter.Start).Take(Filter.PageLength).ToListAsync()
            };
        }
    }
}
