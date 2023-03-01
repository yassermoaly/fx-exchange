using Models.DTOs.Currency;


namespace Models.DTOs.FxTransaction
{
    public record DTOFxTransactionDetail
    {
        public long AccountId { get; set; }
        public short FxTransactionDetailTypeId { get; set; }
        public double Amount { get; set; }
        public double AccountBalancePre { get; set; }
        public double AccountBalancePost { get; set; }
        public DTOCurrency Currency { get; set; } = default!;
        public virtual DTOFxTransactionDetailType FxTransactionDetailType { get; set; } = default!;
    }
}
