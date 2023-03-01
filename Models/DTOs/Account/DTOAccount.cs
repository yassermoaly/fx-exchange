using Models.DTOs.Currency;

namespace Models.DTOs.Account
{
    public record DTOAccount
    {
        public long Id { get; set; }
        public long HolderId { get; set; }
        public double Balance { get; set; }
        public DTOCurrency Currency { get; set; } = default!;
    }
}
