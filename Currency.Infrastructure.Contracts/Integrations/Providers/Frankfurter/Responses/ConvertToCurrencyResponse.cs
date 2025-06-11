namespace Currency.Infrastructure.Contracts.Integrations.Providers.Frankfurter.Responses;

public class ConvertToCurrencyResponse
{
    public string Base { get; set; }
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}