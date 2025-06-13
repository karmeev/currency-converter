namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;

public class ProviderException(string message) : Exception(message)
{
    public Exception OriginalException { get; set; }
    
    public static T ThrowIfProviderNotFoundByRequest<T>(string message, Type requestType)
    {
        throw new ProviderException($"{message}; Request: {requestType.FullName}");
    }
    
    public static T ThrowIfResolutionFailure<T>(string message, Exception originalException)
    {
        throw new ProviderException(message)
        {
            OriginalException = originalException
        };
    }
}