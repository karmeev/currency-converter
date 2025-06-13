using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Currency.Api.ModelBinders;

public class ConvertCurrencyRequestBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var query = context.HttpContext.Request.Query;

        var request = new ConvertCurrencyRequest();
        
        if (!int.TryParse(query["amount"], out var amount))
        {
            context.ModelState.AddModelError("amount", "Invalid or missing amount");
        }

        request.Amount = amount;
        request.FromCurrency = query["from"];
        request.ToCurrency = query["to"];
        context.Result = ModelBindingResult.Success(request);
        return Task.CompletedTask;
    }
}