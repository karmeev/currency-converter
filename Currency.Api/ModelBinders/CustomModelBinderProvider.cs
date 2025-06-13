using Currency.Facades.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Currency.Api.ModelBinders;

public class CustomModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(GetExchangeRateHistoryRequest))
        {
            return new BinderTypeModelBinder(typeof(GetExchangeRateHistoryRequestBinder));
        }
        
        if (context.Metadata.ModelType == typeof(ConvertCurrencyRequest))
        {
            return new BinderTypeModelBinder(typeof(ConvertCurrencyRequestBinder));
        }
        
        return null;
    }
}
