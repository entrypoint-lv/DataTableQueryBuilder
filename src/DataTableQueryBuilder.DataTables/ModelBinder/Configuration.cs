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
        public static Options Options { get; private set; }

        /// <summary>
        /// Static constructor.
        /// Set's default configuration for DataTableQueryBuilder.DataTables.
        /// </summary>
        static Configuration()
        {
            Options = new Options();
        }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        public static void RegisterDataTables(this IServiceCollection services) { services.RegisterDataTables(new Options()); }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        /// <param name="options">DataTableQueryBuilder.DataTables options.</param>
        public static void RegisterDataTables(this IServiceCollection services, Options options) { services.RegisterDataTables(options, new ModelBinder()); }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        /// <param name="requestModelBinder">Request model binder to use when resolving 'DataTablesRequest' models.</param>
        public static void RegisterDataTables(this IServiceCollection services, ModelBinder requestModelBinder) { services.RegisterDataTables(new Options(), requestModelBinder); }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        /// <param name="parseRequestAdditionalParameters">Function to evaluante and parse aditional parameters sent within the request (user-defined parameters).</param>
        /// <param name="parseResponseAdditionalParameters">Indicates whether response aditional parameters parsing is enabled or not.</param>
        public static void RegisterDataTables(this IServiceCollection services, Func<ModelBindingContext, IDictionary<string, object>>? parseRequestAdditionalParameters) { services.RegisterDataTables(new Options(), new ModelBinder(), parseRequestAdditionalParameters); }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="options">DataTableQueryBuilder.DataTables options.</param>
        /// <param name="requestModelBinder">Model binder to use when resolving 'DataTablesRequest' model.</param>
        public static void RegisterDataTables(this IServiceCollection services, Options options, ModelBinder requestModelBinder) { services.RegisterDataTables(options, requestModelBinder, null); }

        /// <summary>
        /// Provides DataTableQueryBuilder.DataTables model binder registration.
        /// </summary>
        /// <param name="services">Service collection for dependency injection.</param>
        /// <param name="options">DataTableQueryBuilder.DataTables options.</param>
        /// <param name="requestModelBinder">Request model binder to use when resolving 'DataTablesRequest' models.</param>
        /// <param name="parseRequestAdditionalParameters">Function to evaluate and parse aditional parameters sent within the request (user-defined parameters).</param>
        public static void RegisterDataTables(this IServiceCollection services, Options options, ModelBinder requestModelBinder, Func<ModelBindingContext, IDictionary<string, object>>? parseRequestAdditionalParameters)
        {
            if (requestModelBinder == null)
                throw new ArgumentNullException("requestModelBinder", "Request model binder for DataTableQueryBuilder.DataTables cannot be null.");

            Options = options ?? throw new ArgumentNullException("options", "Options for DataTableQueryBuilder.DataTables cannot be null.");

            if (parseRequestAdditionalParameters != null)
                requestModelBinder.ParseAdditionalParameters = parseRequestAdditionalParameters;

            // Should be inserted into first position because there is a generic binder which could end up resolving/binding model incorrectly.
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(o => o.ModelBinderProviders.Insert(0, new DataTablesRequestModelBinderProvider(requestModelBinder)));
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
