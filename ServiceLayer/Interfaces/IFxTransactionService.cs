using Models.DTOs.FxTransaction;
using Models.DTOs.General;
namespace ServiceLayer.Interfaces
{
    public interface IFxTransactionService
    {
        Task<GResponse<DTOFxTransaction>> Create(DTOCreateFxTransactionRequest Posted);
        Task<FilterResponse<DTOFxTransaction>> Filter(FilterRequest<DTOFxTransactionFilter> Filter);
    }
}
