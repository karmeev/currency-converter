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
}