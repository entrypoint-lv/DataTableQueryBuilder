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
        /// Indicates if model binder is used for request model.
        /// </summary>
        public bool UseRequestModelBinder { get; private set; }

        /// <summary>
        /// Get's a function to evaluante and parse aditional parameters sent within the request (user-defined parameters).
        /// </summary>
        public Func<HttpContext, IDictionary<string, object>>? ParseRequestAdditionalParameters { get; private set; }

        /// <summary>
        /// Gets default page length when parameter is not set.
        /// </summary>
        public int DefaultPageLength { get; private set; }

        /// <summary>
        /// Gets an indicator if draw parameter should be validated.
        /// </summary>
        public bool IsDrawValidationEnabled { get; private set; }

        /// <summary>
        /// Gets the request name convention to be used when resolving request parameters.
        /// </summary>
        public RequestNameConvention RequestNameConvention { get; private set; }

        /// <summary>
        /// Gets the response name convention to be used when serializing response elements.
        /// </summary>
        public ResponseNameConvention ResponseNameConvention { get; private set; }

        /// <summary>
        /// Sets the default page length to be used when request parameter is not set.
        /// Page length is set to 20 by default.
        /// </summary>
        /// <param name="defaultPageLength">The new default page length to be used.</param>
        /// <returns></returns>
        public Options SetDefaultPageLength(int defaultPageLength) { DefaultPageLength = defaultPageLength; return this; }

        /// <summary>
        /// Enables draw validation.
        /// Draw validation is enabled by default.
        /// </summary>
        /// <returns></returns>
        public Options EnableDrawValidation() { IsDrawValidationEnabled = true; return this; }

        /// <summary>
        /// Disables draw validation.
        /// As stated by jQuery DataTables, draw validation should not be disabled unless explicitly required.
        /// </summary>
        /// <returns></returns>
        public Options DisableDrawValidation() { IsDrawValidationEnabled = false; return this; }

        /// <summary>
        /// Creates a new 'Option' instance.
        /// </summary>
        public Options(bool useModelBinder) : this (20, true, useModelBinder)
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
            IsDrawValidationEnabled = enableDrawValidation;
            UseRequestModelBinder = useRequestModelBinder;

            RequestNameConvention = new RequestNameConvention();
            ResponseNameConvention = new ResponseNameConvention();
        }
    }
}
