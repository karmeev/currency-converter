namespace Currency.Api.Exceptions;

public class StartupException(string message) : Exception(message)
{
    public static T ThrowIfConfigurationIncorrect<T>()
    {
        throw new StartupException("Configuration binding failure");
    }
}