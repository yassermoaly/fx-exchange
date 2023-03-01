using Models.DTOs.Holder;
using Models.Data;
using Models.DTOs.General;

namespace ServiceLayer.Interfaces
{
    public interface IHolderService
    {
        Task<FilterResponse<DTOHolder>> Filter(FilterRequest<string> Filter);
        Task<Holder> Get(long Id);
    }
}
