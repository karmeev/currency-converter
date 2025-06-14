namespace Currency.Facades.Contracts.Exceptions;

public class ValidationException(string message) : Exception(message)
{
    private const string DefaultMessage = "Validation failed.";
    
    public string[] ErrorMessages { get; private set; }
    
    public static T Throw<T>(string message)
    {
        if (string.IsNullOrEmpty(message))
            message = DefaultMessage;
        
        throw new ValidationException(message);
    }
    
    public static T Throw<T>(string message, string[] errors)
    {
        if (errors.Length == 0)
        {
            return Throw<T>(message);
        }
        
        throw new ValidationException(message)
        {
            ErrorMessages = errors
        };
    }
}