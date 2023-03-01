namespace Models.DTOs.Currency
{
    public record DTOCurrency
    {
        public short Id { get; set; }
        public string Name { get; set; } = default!;
        public string ISOCode { get; set; } = default!;
    }
}
