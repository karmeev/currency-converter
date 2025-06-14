namespace Currency.Facades.Contracts.Exceptions;

public class ValidationException(string message) : Exception(message)
{
    private const string DefaultMessage = "Validation failed.";
    
    public string[] ErrorMessages { get; private set; }
    
    public static void Throw(string message)
    {
        if (string.IsNullOrEmpty(message))
            message = DefaultMessage;
        
        throw new ValidationException(message);
    }
    
    public static void Throw(string message, string[] errors)
    {
        if (errors.Length == 0)
        {
            Throw(message);
        }
        
        throw new ValidationException(message)
        {
            ErrorMessages = errors
        };
    }
}