using DataAccessLayer;
using DataAccessLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;
using Models.DTOs.FxTransaction;
using ServiceLayer.Interfaces;

namespace ServiceLayer
{
    public class FxTransactionDetailTypeService : IFxTransactionDetailTypeService
    {     
        private static ICollection<FxTransactionDetailType> _fxTransactionDetailTypes = new HashSet<FxTransactionDetailType>();      
        public FxTransactionDetailType Get(short Id)
        {
            return _fxTransactionDetailTypes.FirstOrDefault(r => r.Id == Id) ?? throw new ApplicationException("FxTransactionDetailType is not found");
        }
        public async Task Load(IFxTransactionDetailTypeRepository FxTransactionDetailTypeRepository)
        {           
            _fxTransactionDetailTypes = await FxTransactionDetailTypeRepository.GetAllAsync();
        }
    }
}
