using Models.Data;
using Models.DTOs.Holder;

namespace Models.DTOs.FxTransaction
{
    public record DTOFxTransaction
    {
        public DTOFxTransaction()
        {
            FxTransactionDetails = new HashSet<DTOFxTransactionDetail>();
        }
        public long Id { get; set; }
        public long HolderId { get; set; }
        public double FxRate { get; set; }
        public double Amount { get; set; }
        public FxTransactionFixedSideEnum FixedSide { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual DTOHolder Holder { get; set; } = default!;

        public ICollection<DTOFxTransactionDetail> FxTransactionDetails { get; set; }
    }
}
