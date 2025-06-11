namespace Currency.Facades.Validators.Results;

public readonly struct ValidationResult(bool isValid, string message)
{
    public bool IsValid { get; } = isValid;
    public string Message { get; } = message;

    public static ValidationResult Success => new(true, string.Empty);
}