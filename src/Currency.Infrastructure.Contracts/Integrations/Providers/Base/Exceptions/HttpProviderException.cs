namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;

public class HttpProviderException : Exception
{
    public HttpProviderException(string message, Exception innerException = null)
        : base(message, innerException)
    { }
}