using Currency.Facades.Validators.Results;

namespace Currency.Facades.Validators;

internal static class AuthValidator
{
    public static ValidationResult Validate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new ValidationResult(false, "Username and password are required.");
        }

        return ValidationResult.Success;
    }
}