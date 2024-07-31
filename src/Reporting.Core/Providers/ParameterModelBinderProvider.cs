namespace Reporting.Core.Providers
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Reporting.Core.Binders;

    public class ParameterModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Dictionary<string, object>))
            {
                return new ParameterModelBinder();
            }

            return null;
        }
    }
}
