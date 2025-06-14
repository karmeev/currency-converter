using System.Globalization;
using System.Text;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Validators.Results;

namespace Currency.Facades.Validators;

public static class ExchangeRatesValidator
{
    public static ValidationResult ValidateRequest(GetExchangeRateHistoryRequest request, out string[] errors)
    {
        var failedResult = new ValidationResult(false, "Invalid request to get history");
        
        var validationErrors = new List<string>();
        if (request.Page < 1 || request.PageSize < 1)
        {
            validationErrors.Add("Page size must be greater than or equal to 1.");
        }

        if (!IsValidCurrency(request.Currency))
        {
            validationErrors.Add("Currency not valid.");
        }

        if (request.StartDate > request.EndDate)
        {
            validationErrors.Add("The end date must be after or equal the start date.");
        }
        
        if (request.EndDate > DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            validationErrors.Add("The end date can not be in future.");
        }
        
        errors = validationErrors.ToArray();
        if (validationErrors.Count != 0)
        {
            return failedResult;
        }

        return ValidationResult.Success;
    }
    
    public static ValidationResult ValidateRequest(ConvertCurrencyRequest request, out string[] errors)
    {
        var bannedCurrencies = new HashSet<string> { "TRY", "PLN", "THB", "MXN" };
        
        var failedResult = new ValidationResult(false, "Invalid request for currency conversion");
        
        var validationErrors = new List<string>();
        if (request.Amount <= 0)
        {
            validationErrors.Add("Amount must be greater than or equal to 0.");
        }

        var hasBanned = false;
        if (bannedCurrencies.Contains(request.FromCurrency))
        {
            validationErrors.Add($"{request.FromCurrency}: currency are not allowed.");
            hasBanned = true;
        }

        if (bannedCurrencies.Contains(request.ToCurrency))
        {
            validationErrors.Add($"{request.ToCurrency}: currency are not allowed.");
            hasBanned = true;
        }

        if (!hasBanned)
        {
            if (!IsValidCurrency(request.ToCurrency))
            {
                validationErrors.Add($"Invalid currency: {request.ToCurrency}.");
            }

            if (!IsValidCurrency(request.FromCurrency))
            {
                validationErrors.Add($"Invalid currency: {request.FromCurrency}");
            }
        }

        errors = validationErrors.ToArray();
        if (errors.Length != 0)
        {
            return failedResult;
        }

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateRequest(string currency)
    {
        if (!IsValidCurrency(currency))
        {
            return new ValidationResult(false, $"Invalid currency: {currency}.");
        }
        
        return ValidationResult.Success;
    }
    
    private static bool IsValidCurrency(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            return false;

        try
        {
            var cultures = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(c => new RegionInfo(c.LCID));

            return cultures.Any(r => r.ISOCurrencySymbol.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

}