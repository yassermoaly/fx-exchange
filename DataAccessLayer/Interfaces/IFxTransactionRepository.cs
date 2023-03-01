using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;

namespace DataAccessLayer.Interfaces
{
    public interface IFxTransactionRepository : IGenericRepository<FxTransaction>
    {
        Task<FilterResponse<FxTransaction>> Filter(FilterRequest<DTOFxTransactionFilter> Filter);
    }
}
