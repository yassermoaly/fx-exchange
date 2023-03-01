namespace Models.Data
{
    public record Account
    {
        public long Id { get; set; }
        public long HolderId { get; set; }
        public double Balance { get; set; }
        public short CurrencyId { get; set; }
        public TimeSpan? RowVersion { get; set; }
        public virtual Holder Holder { get; set; } = default!;
        public virtual Currency Currency { get; set; } = default!;
    }
}
