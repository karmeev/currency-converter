namespace Currency.Facades.Validators.Results;

public struct ValidationResult(bool isValid, string message)
{
    public bool IsValid = isValid;
    public string Message = message;

    public static ValidationResult Success => new(true, string.Empty);
}