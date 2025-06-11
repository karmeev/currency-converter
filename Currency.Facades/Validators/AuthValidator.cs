using Currency.Facades.Validators.Results;

namespace Currency.Facades.Validators;

internal class AuthValidator : IAuthValidator
{
    public ValidationResult Validate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new ValidationResult(false, "Username is required");

        if (string.IsNullOrWhiteSpace(password))
            return new ValidationResult(false, "Password is required");

        return ValidationResult.Success;
    }
}