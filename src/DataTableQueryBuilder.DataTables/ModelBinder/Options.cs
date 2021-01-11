namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a configuration object for DataTableQueryBuilder.DataTables.
    /// </summary>
    public class Options
    {
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
        public Options() : this (20, true)
        { }

        /// <summary>
        /// Creates a new 'Option' instance.
        /// </summary>
        /// <param name="defaultPageLength">Default page length to be used.</param>
        /// <param name="enableDrawValidation">Indicates if draw validation will be enabled by default or not.</param>
        public Options(int defaultPageLength, bool enableDrawValidation)
        {
            DefaultPageLength = defaultPageLength;
            IsDrawValidationEnabled = enableDrawValidation;

            RequestNameConvention = new RequestNameConvention();
            ResponseNameConvention = new ResponseNameConvention();
        }
    }
}
