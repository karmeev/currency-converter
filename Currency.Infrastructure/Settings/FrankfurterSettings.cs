namespace Currency.Infrastructure.Settings;

public class FrankfurterSettings
{
    public string BaseAddress
    {
        get => BaseAddressUri?.ToString();
        set => BaseAddressUri = new Uri(value);
    }

    public Uri BaseAddressUri { get; private set; }

    public int TimeoutSeconds { get; set; } = 10;
    public int RetryCount { get; set; } = 3;
    public int RetryExponentialIntervalSeconds { get; set; } = 5;
    public int CircuitBreakerDurationBreakSeconds { get; set; } = 30;
    public int CircuitBreakerMaxExceptions { get; set; } = 6;
}