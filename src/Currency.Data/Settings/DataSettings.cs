using Microsoft.Extensions.Options;

namespace Currency.Data.Settings;

public class DataSettings(IOptionsMonitor<CacheSettings> monitor)
{
    public TimeSpan ExchangeRatesTtl => monitor.CurrentValue.ExchangeRatesTtl;
    public TimeSpan ExchangeRatesHistoryTtl => monitor.CurrentValue.ExchangeRatesHistoryTtl;
    public TimeSpan ConversionResultTtl => monitor.CurrentValue.ConversionResultTtl;
}
