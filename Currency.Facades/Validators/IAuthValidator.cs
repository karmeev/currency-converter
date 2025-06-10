using Currency.Facades.Validators.Results;

namespace Currency.Facades.Validators;

internal interface IAuthValidator
{
    ValidationResult Validate(string username, string password);
}