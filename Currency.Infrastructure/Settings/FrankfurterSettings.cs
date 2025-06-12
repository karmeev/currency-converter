namespace Currency.Infrastructure.Settings;

public class FrankfurterSettings
{
    private Uri _cachedBaseAddress;
    public string BaseAddress
    {
        get => _cachedBaseAddress?.ToString();
        set => _cachedBaseAddress = new Uri(value);
    }

    public Uri BaseAddressUri => _cachedBaseAddress;
    public int TimeoutSeconds { get; set; } = 10;
    public int RetryCount { get; set; } = 3;
    public int RetryExponentialIntervalSeconds { get; set; } = 5;
    public int CircuitBreakerDurationBreakSeconds { get; set; } = 30;
    public int CircuitBreakerMaxExceptions { get; set; } = 6;
}