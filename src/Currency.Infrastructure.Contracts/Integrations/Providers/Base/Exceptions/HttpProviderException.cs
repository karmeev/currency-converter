using System.Net;

namespace Currency.Infrastructure.Contracts.Integrations.Providers.Base.Exceptions;

public class HttpProviderException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpProviderException(string message, HttpStatusCode statusCode, Exception innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}