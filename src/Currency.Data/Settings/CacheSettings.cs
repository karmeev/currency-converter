namespace Currency.Data.Settings;

public class CacheSettings
{
    public TimeSpan ExchangeRatesTtl { get; set; }
    public TimeSpan ExchangeRatesHistoryTtl { get; set; }
    public TimeSpan ConversionResultTtl { get; set; }
}