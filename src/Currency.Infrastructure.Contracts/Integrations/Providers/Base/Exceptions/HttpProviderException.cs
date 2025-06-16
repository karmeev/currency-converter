namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;

public class HttpProviderException : Exception
{
    public HttpProviderException(string message, Exception innerException = null)
        : base(message, innerException)
    { }

    public static T Throw<T>(string message, Exception innerException = null)
    {
        throw new HttpProviderException(message, innerException);
    }
}