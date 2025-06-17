namespace Currency.Facades.Validators.Results;

public struct ValidationResult(bool isValid, string message)
{
    public readonly bool IsValid = isValid;
    public readonly string Message = message;

    public static ValidationResult Success => new(true, string.Empty);
}