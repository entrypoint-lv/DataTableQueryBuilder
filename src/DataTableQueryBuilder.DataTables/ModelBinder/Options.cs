using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System;

using Microsoft.AspNetCore.Http;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a configuration object for DataTableQueryBuilder.DataTables.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets an indicator if model binder should be used for request model.
        /// </summary>
        public bool UseRequestModelBinder { get; set; }

        /// <summary>
        /// Gets or sets a function to evaluante and parse aditional parameters sent within the request (user-defined parameters).
        /// </summary>
        public Func<HttpContext, IDictionary<string, object>>? ParseRequestAdditionalParameters { get; set; }

        /// <summary>
        /// Gets or sets default page length when parameter is not set.
        /// </summary>
        public int DefaultPageLength { get; set; }

        /// <summary>
        /// Gets or sets an indicator if draw parameter should be validated.
        /// </summary>
        public bool EnableDrawValidation { get; set; }

        /// <summary>
        /// Gets or sents the request name convention to be used when resolving request parameters.
        /// </summary>
        public RequestNameConvention RequestNameConvention { get; set; }

        /// <summary>
        /// Gets or sets the response name convention to be used when serializing response elements.
        /// </summary>
        public ResponseNameConvention ResponseNameConvention { get; set; }

        /// <summary>
        /// Creates a new 'Option' instance.
        /// </summary>
        public Options() : this (20, true, true)
        { }

        /// <summary>
        /// Creates a new 'Option' instance.
        /// </summary>
        /// <param name="defaultPageLength">Default page length to be used.</param>
        /// <param name="enableDrawValidation">Indicates if draw validation will be enabled by default or not.</param>
        /// <param name="useRequestModelBinder">Indicates if model binder should be created and registered for request model.</param>
        public Options(int defaultPageLength, bool enableDrawValidation, bool useRequestModelBinder)
        {
            DefaultPageLength = defaultPageLength;
            EnableDrawValidation = enableDrawValidation;
            UseRequestModelBinder = useRequestModelBinder;

            RequestNameConvention = new RequestNameConvention();
            ResponseNameConvention = new ResponseNameConvention();
        }
    }
}
