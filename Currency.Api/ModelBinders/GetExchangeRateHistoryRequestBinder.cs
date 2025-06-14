using System.Globalization;
using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Currency.Api.ModelBinders;

public class GetExchangeRateHistoryRequestBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var query = context.HttpContext.Request.Query;

        var request = new GetHistoryRequest
        {
            Page = int.TryParse(query["page"], out var page) ? page : 1,
            PageSize = int.TryParse(query["size"], out var size) ? size : 10,
            Currency = query["base"]
        };

        if (!TryParseFlexibleDateTime(query["startDate"], out var startDate))
        {
            context.ModelState.AddModelError("startDate", "Invalid or missing startDate");
        }

        if (!TryParseFlexibleDateTime(query["endDate"], out var endDate))
        {
            context.ModelState.AddModelError("endDate", "Invalid or missing endDate");
        }

        if (!context.ModelState.IsValid)
            return Task.CompletedTask;

        request.StartDate = startDate;
        request.EndDate = endDate;

        context.Result = ModelBindingResult.Success(request);
        return Task.CompletedTask;
    }

    private static bool TryParseFlexibleDateTime(string value, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
            return true;
        
        if (DateOnly.TryParse(value, out var dateOnly))
        {
            result = dateOnly.ToDateTime(TimeOnly.MinValue);
            return true;
        }

        return false;
    }
}

