using Models.Data;
using System.ComponentModel;

namespace Models.DTOs.Rate
{
    public class DTOConversionRateRequest
    {
        [DefaultValue("EUR")]
        public string BuyCurrency { get; set; } = default!;
        [DefaultValue("USD")]
        public string SellCurrency { get; set; } = default!;
        [DefaultValue(1)]
        public long HolderId { get; set; }
        [DefaultValue(FxTransactionFixedSideEnum.Sell)]
        public FxTransactionFixedSideEnum FixedSide { get; set; }
        [DefaultValue(50)]
        public double Amount { get; set; }
    }
}
