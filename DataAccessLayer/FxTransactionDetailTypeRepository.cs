using DataAccessLayer.Interfaces;
using Models.Data;


namespace DataAccessLayer
{
    public class FxTransactionDetailTypeRepository : GenericRepository<FxTransactionDetailType>, IFxTransactionDetailTypeRepository
    {
        public FxTransactionDetailTypeRepository(FxExchangeDBContext context) : base(context)
        {
        }
    }
}
