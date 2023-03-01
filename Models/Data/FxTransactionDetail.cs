using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Data
{
    public record FxTransactionDetail
    {
        public long Id { get; set; }
        public long FxTransactionId { get; set; }
        public long AccountId { get; set; }
        public short FxTransactionDetailTypeId { get; set; }
        public double Amount { get; set; }
        public double AccountBalancePre { get; set; }
        public double AccountBalancePost { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual FxTransactionDetailType FxTransactionDetailType { get; set; } = default!;
        public virtual FxTransaction FxTransaction { get; set; } = default!;
        public virtual Account Account { get; set; } = default!;
    }
}
