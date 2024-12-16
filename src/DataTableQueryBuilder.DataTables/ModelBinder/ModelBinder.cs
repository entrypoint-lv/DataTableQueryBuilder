using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a model binder for DataTables request element.
    /// </summary>
    public class ModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds request data/parameters/values into a 'DataTablesRequest' element.
        /// </summary>
        /// <param name="bindingContext">Binding context for data/parameters/values.</param>
        /// <returns>An DataTablesRequest object or null if binding was not possible.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return Task.Factory.StartNew(() => BindModel(bindingContext, Configuration.Options));
        }

        /// <summary>
        /// Binds request data/parameters/values into a 'DataTablesRequest' element.
        /// </summary>
        /// <param name="controllerContext">Controller context for execution.</param>
        /// <param name="bindingContext">Binding context for data/parameters/values.</param>
        /// <param name="options">DataTableQueryBuilder.DataTables global options.</param>
        /// <returns>An DataTablesRequest object or null if binding was not possible.</returns>
        public virtual void BindModel(ModelBindingContext bindingContext, Options options)
        {
            if (!bindingContext.ModelType.Equals(typeof(DataTableRequest)))
                return;

            var model = DataTableRequest.BindAsync(bindingContext.HttpContext).Result;

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
