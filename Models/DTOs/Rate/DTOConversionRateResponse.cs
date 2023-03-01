using Models.Data;

namespace Models.DTOs.Rate
{
    public record DTOConversionRateResponse
    {
        public Guid ReferenceId { get; set; }
        public string ConversionRateRequestId { get; set; } = default!;
        public string BuyCurrency { get; set; } = default!;
        public string SellCurrency { get; set; } = default!;
        public double Amount { get; set; }
        public FxTransactionFixedSideEnum FixedSide { get; set; }  = default!;
        public long HolderId { get; set; }

        public double BuyAmount { get; set; }

        public double SellAmount { get; set; }

        public double ConversionRate { get; set; }
    }
}
