namespace Currency.Services.Application.Settings;

public class WorkerSettings
{
    public int Bandwidth { get; set; } = 10;
    public int ConsumeDelayInMilliseconds { get; set; } = 10;
    public int ExchangeRatesHistoryWorkers { get; init; } = 1;
    public int CurrencyConversionWorkers { get; init; } = 1;
    public int ExchangeRatesWorkers { get; init; } = 1;
}