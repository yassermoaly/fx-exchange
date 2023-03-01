namespace Models.DTOs.FxTransaction
{
    public record DTOCreateFxTransactionRequest
    {
        public string ConversionRateRequestId { get; set; } = default!;
    }
}
