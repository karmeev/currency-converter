namespace Currency.Data.Contracts.Exceptions;

public class ConcurrencyException(string message, string lockId = "empty") : Exception(message)
{
    public string LockId { get; init; } = lockId;
    
    public static void Throw(string message)
    {
        throw new ConcurrencyException(message);
    }
    
    public static void ThrowIfExists(string message, string lockId)
    {
        throw new ConcurrencyException(message, lockId);
    }
}