using Reporting.Core.Helpers;

namespace Reporting.Core.Binders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class ParameterModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var model = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var query = bindingContext.HttpContext.Request.Query;

            foreach (var kvp in query)
            {
                var key = kvp.Key;
                var value = kvp.Value.ToString();
                model[key] = ObjectHelpers.ConvertToObject(value);
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}
