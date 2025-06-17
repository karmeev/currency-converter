namespace Currency.Data.Contracts.Exceptions;

public class NotFoundException(string message, Exception innerException = null) : Exception(message)
{
    public static T Throw<T>(string message, Exception innerException = null)
    {
        throw new NotFoundException(message, innerException);
    }
}