namespace Integration.Interfaces
{    
    public interface IFxExchangeRateWebService
    {
        Task<double> GetRate(string From, string To);
    }
}
