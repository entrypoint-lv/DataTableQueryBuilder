using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Handles DataTableQueryBuilder.DataTables registration and holds default (global) configuration options.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Get's DataTableQueryBuilder.DataTables runtime options for server-side processing.
        /// </summary>
        public static Options Options { get; private set; } = default!;

        /// <summary>
        /// Adds DataTableQueryBuilder.DataTables registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        public static void AddDataTables(this IServiceCollection services) { services.AddDataTables(true); }

        /// <summary>
        /// Adds DataTableQueryBuilder.DataTables registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        public static void AddDataTables(this IServiceCollection services, bool useModelBinder) { services.AddDataTables(new Options(useModelBinder)); }

        /// <summary>
        /// Adds DataTableQueryBuilder.DataTables registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        /// <param name="options">DataTableQueryBuilder.DataTables options.</param>
        /// <param name="requestModelBinder">Request model binder to use when resolving 'DataTablesRequest' models.</param>
        /// <param name="parseRequestAdditionalParameters">Function to evaluate and parse aditional parameters sent within the request (user-defined parameters).</param>
        public static void AddDataTables(this IServiceCollection services, Options options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options), "Options for DataTableQueryBuilder.DataTables cannot be null.");

            if (options.UseRequestModelBinder)
                // Should be inserted into first position because there is a generic binder which could end up resolving/binding model incorrectly.
                services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(o => o.ModelBinderProviders.Insert(0, new DataTablesRequestModelBinderProvider(new ModelBinder())));
        }

        internal class DataTablesRequestModelBinderProvider : IModelBinderProvider
        {
            public IModelBinder? ModelBinder { get; private set; }

            public DataTablesRequestModelBinderProvider() { }

            public DataTablesRequestModelBinderProvider(IModelBinder modelBinder) { ModelBinder = modelBinder; }

            public IModelBinder? GetBinder(ModelBinderProviderContext context)
            {
                if (IsBindable(context.Metadata.ModelType))
                {
                    if (ModelBinder == null) ModelBinder = new ModelBinder();
                    return ModelBinder;
                }
                
                return null;
            }

            private bool IsBindable(Type type) { return type.Equals(typeof(DataTableRequest)); }
        }
    }
}
