using Models.Data;
using Models.DTOs.General;

namespace DataAccessLayer.Interfaces
{
    public interface IHolderRepository:  IGenericRepository<Holder>
    {
        Task<FilterResponse<Holder>> Filter(FilterRequest<string> Filter);
    }
}
