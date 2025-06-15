namespace Currency.Services.Contracts.Application.Exceptions;

public class CurrencyNotFoundException(string message) : Exception
{
    private const string DefaultMessage = "Currency not found";
    
    public static T Throw<T>(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            message = DefaultMessage;
        }
        
        throw new CurrencyNotFoundException(message);
    }
}