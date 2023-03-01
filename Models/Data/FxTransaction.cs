namespace Models.Data
{
    public record FxTransaction
    {
        public FxTransaction()
        {
            FxTransactionDetails = new HashSet<FxTransactionDetail>();
        }
        public long Id { get; set; }
        public long HolderId { get; set; }
        public short FxTransactionFixedSideId { get; set; }
        public double FxRate { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual Holder Holder { get; set; } = default!;
        public double Amount { get; set; }

        public virtual FxTransactionFixedSide FxTransactionFixedSide { get; set; } = default!;

        public ICollection<FxTransactionDetail> FxTransactionDetails { get; set; }
    }
}
