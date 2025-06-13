using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Currency.Api.ModelBinders;

public class GetExchangeRateHistoryRequestBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var query = context.HttpContext.Request.Query;

        var request = new GetExchangeRateHistoryRequest();

        request.Page = int.TryParse(query["page"], out var page) ? page : 1;
        request.PageSize = int.TryParse(query["size"], out var size) ? size : 10;
        request.Currency = query["base"];

        if (!DateOnly.TryParse(query["startDate"], out var startDate))
        {
            context.ModelState.AddModelError("startDate", "Invalid or missing startDate");
        }

        if (!DateOnly.TryParse(query["endDate"], out var endDate))
        {
            context.ModelState.AddModelError("endDate", "Invalid or missing endDate");
        }
        
        if (!context.ModelState.IsValid)
        {
            return Task.CompletedTask;
        }

        request.StartDate = startDate;
        request.EndDate = endDate;

        context.Result = ModelBindingResult.Success(request);
        return Task.CompletedTask;
    }
}
