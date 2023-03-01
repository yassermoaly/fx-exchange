namespace Models.DTOs.FxTransaction
{
    public record DTOFxTransactionDetailType
    {
        public short Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
