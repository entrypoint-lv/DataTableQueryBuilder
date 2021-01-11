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
            return Task.Factory.StartNew(() => BindModel(bindingContext, Configuration.Options, ParseAdditionalParameters));
        }

        /// <summary>
        /// Binds request data/parameters/values into a 'DataTablesRequest' element.
        /// </summary>
        /// <param name="controllerContext">Controller context for execution.</param>
        /// <param name="bindingContext">Binding context for data/parameters/values.</param>
        /// <param name="options">DataTableQueryBuilder.DataTables global options.</param>
        /// <returns>An DataTablesRequest object or null if binding was not possible.</returns>
        public virtual void BindModel(ModelBindingContext bindingContext, Options options, Func<ModelBindingContext, IDictionary<string, object>>? parseAditionalParameters)
        {
            if (!bindingContext.ModelType.Equals(typeof(DataTablesRequest)))
                return;

            // Binding is set to a null model to avoid unexpected errors.
            if (options == null || options.RequestNameConvention == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var values = bindingContext.ValueProvider;
            var names = options.RequestNameConvention;

            // Accordingly to DataTables docs, it is recommended to receive/return draw casted as int for security reasons.
            int draw = 0;
            if (options.IsDrawValidationEnabled && !Parse(values.GetValue(names.Draw), out draw))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            Parse(values.GetValue(names.StartRecordNumber), out int start);

            Parse(values.GetValue(names.PageSize), out int pageSize);
            if (pageSize == 0)
                pageSize = options.DefaultPageLength;

            Parse(values.GetValue(names.SearchValue), out string? searchValue);

            Parse(values.GetValue(names.IsSearchRegex), out bool searchRegex);

            // Parse columns & column sorting.
            var columns = ParseColumns(values, names);
            ParseSorting(columns, values, names);

            var aditionalParameters = parseAditionalParameters == null ? null : parseAditionalParameters(bindingContext);

            var model = new DataTablesRequest(draw, start, pageSize, new Search(searchValue, searchRegex), columns, aditionalParameters);
            {
                bindingContext.Result = ModelBindingResult.Success(model);
                return;
            }
        }

        /// <summary>
        /// Provides custom aditional parameters processing for your request.
        /// You have to implement this to populate 'DataTablesRequest' object with aditional (user-defined) request values.
        /// </summary>
        public Func<ModelBindingContext, IDictionary<string, object>>? ParseAdditionalParameters;

        /// <summary>
        /// Parse column collection.
        /// </summary>
        /// <param name="values">Request parameters.</param>
        /// <param name="names">Name convention for request parameters.</param>
        /// <returns></returns>
        private static IEnumerable<Column> ParseColumns(IValueProvider values, RequestNameConvention names)
        {
            var columns = new List<Column>();

            int counter = 0;

            while (true)
            {
                // Parses Field value.
                if (!Parse(values.GetValue(String.Format(names.ColumnField, counter)), out string columnField))
                    break;

                // Parses Name value.
                Parse(values.GetValue(String.Format(names.ColumnName, counter)), out string? columnName);

                // Parses Orderable value.
                bool columnSortable = true;
                Parse(values.GetValue(String.Format(names.IsColumnSortable, counter)), out columnSortable);

                // Parses Searchable value.
                bool columnSearchable = true;
                Parse(values.GetValue(String.Format(names.IsColumnSearchable, counter)), out columnSearchable);

                // Parsed Search value.
                Parse(values.GetValue(String.Format(names.ColumnSearchValue, counter)), out string? columnSearchValue);

                // Parses IsRegex value.
                Parse(values.GetValue(String.Format(names.IsColumnSearchRegex, counter)), out bool columnSearchRegex);

                var column = new Column(columnName, columnField, columnSearchable, columnSortable, new Search(columnSearchValue, columnSearchRegex));

                columns.Add(column);

                counter++;
            }

            return columns;
        }

        /// <summary>
        /// Parse sort collection.
        /// </summary>
        /// <param name="columns">Column collection to use when parsing sort.</param>
        /// <param name="values">Request parameters.</param>
        /// <param name="names">Name convention for request parameters.</param>
        /// <returns></returns>
        private static void ParseSorting(IEnumerable<Column> columns, IValueProvider values, RequestNameConvention names)
        {
            for (int i = 0; i < columns.Count(); i++)
            {
                if (!Parse(values.GetValue(String.Format(names.SortField, i)), out int sortField))
                    break;

                var column = columns.ElementAt(sortField);

                if (!column.IsSortable)
                    continue;
                
                Parse(values.GetValue(String.Format(names.SortDirection, i)), out string sortDirection);

                column.SetSort(i, sortDirection);
            }
        }

        /// <summary>
        /// Parses a possible raw value and transforms into a strongly-typed result.
        /// </summary>
        /// <typeparam name="ElementType">The expected type for result.</typeparam>
        /// <param name="value">The possible request value.</param>
        /// <param name="result">Returns the parsing result or default value for type is parsing failed.</param>
        /// <returns>True if parsing succeeded, False otherwise.</returns>
        private static bool Parse<ElementType>(ValueProviderResult value, out ElementType result)
        {
            result = default!;

            if (value == null)
                return false;
            
            if (string.IsNullOrWhiteSpace(value.FirstValue))
                return false;

            try
            {
                result = (ElementType)Convert.ChangeType(value.FirstValue, typeof(ElementType));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
