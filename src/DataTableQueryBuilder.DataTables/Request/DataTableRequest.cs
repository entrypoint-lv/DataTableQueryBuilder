using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a DataTables request.
    /// </summary>
    public class DataTableRequest : IDataTableRequest
    {
        /// <summary>
        /// Gets draw counter.
        /// This is used by DataTables to ensure that the Ajax returns from server-side procesing request are drawn in sequence by DataTables.
        /// </summary>
        public int Draw { get; }

        /// <summary>
        /// Gets paging first record indicator.
        /// This is the start point in the current data set (zero index based).
        /// </summary>
        public int StartRecordNumber { get; }

        /// <summary>
        /// Gets the number of records that the table can display in the current draw.
        /// It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
        /// A value &lt;= 0 to indicate that all records should be returned (although that negates any benefits of server-side processing!).
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets DataTables column collection (from client-side).
        /// </summary>
        public IEnumerable<Column> Columns { get; }

        /// <summary>
        /// Gets the search value to be applied to all columns that have global search enabled.
        /// </summary>
        public Search Search { get; }

        /// <summary>
        /// Gets the search value to be applied to all columns that have global search enabled.
        /// </summary>
        public string? SearchValue => Search.Value;

        /// <summary>
        /// Gets searchable fields.
        /// </summary>
        public Dictionary<string, string?> SearchableFields => Columns.Where(c => c.Field != null && c.IsSearchable).ToDictionary(c => c.Field!, c => c.Search?.Value);

        /// <summary>
        /// Gets sortable fields.
        /// </summary>
        public Dictionary<string, Sort?> SortableFields => Columns.Where(c => c.Field != null && c.IsSortable).ToDictionary(c => c.Field!, c => c.Sort);

        /// <summary>
        /// Gets the user-defined collection of parameters.
        /// </summary>
        public IDictionary<string, object>? AdditionalParameters { get; }

        public DataTableRequest(int draw, int start, int pagesize, Search search, IEnumerable<Column> columns) : this(draw, start, pagesize, search, columns, null)
        { }

        public DataTableRequest(int draw, int start, int pageSize, Search search, IEnumerable<Column> columns, IDictionary<string, object>? additionalParameters)
        {
            Draw = draw;
            StartRecordNumber = start;
            PageSize = pageSize;
            Search = search;
            Columns = columns;
            AdditionalParameters = additionalParameters;
        }

        public static ValueTask<DataTableRequest?> BindAsync(HttpContext context, ParameterInfo? parameter = null)
        {
            var options = Configuration.Options;

            if (options == null)
                throw new ArgumentNullException(nameof(options), "Options for DataTableQueryBuilder.DataTables cannot be null.");

            var values = context.Request.Form;
            var names = options.RequestNameConvention;

            // Accordingly to DataTables docs, it is recommended to receive/return draw casted as int for security reasons.
            int draw = 0;
            if (options.EnableDrawValidation && !Parse(values[names.Draw], out draw))
            {
                return ValueTask.FromResult<DataTableRequest?>(null);
            }

            Parse(values[names.StartRecordNumber], out int start);

            Parse(values[names.PageSize], out int pageSize);
            if (pageSize == 0)
                pageSize = options.DefaultPageLength;

            Parse(values[names.SearchValue], out string? searchValue);

            Parse(values[names.IsSearchRegex], out bool searchRegex);

            // Parse columns & column sorting.
            var columns = ParseColumns(values, names);
            ParseSorting(columns, values, names);

            var aditionalParameters = options.ParseRequestAdditionalParameters == null ? null : options.ParseRequestAdditionalParameters(context);

            var model = new DataTableRequest(draw, start, pageSize, new Search(searchValue, searchRegex), columns, aditionalParameters);

            return ValueTask.FromResult<DataTableRequest?>(model);
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
        private static IEnumerable<Column> ParseColumns(IFormCollection values, RequestNameConvention names)
        {
            var columns = new List<Column>();

            int counter = 0;

            while (true)
            {
                // Parses Name value.
                if (!Parse(values[String.Format(names.ColumnName, counter)], out string? columnName))
                    break;

                // Parses Field value.
                Parse(values[String.Format(names.ColumnField, counter)], out string? columnField);

                // Parses Orderable value.
                bool columnSortable = true;
                Parse(values[String.Format(names.IsColumnSortable, counter)], out columnSortable);

                // Parses Searchable value.
                bool columnSearchable = true;
                Parse(values[String.Format(names.IsColumnSearchable, counter)], out columnSearchable);

                // Parsed Search value.
                Parse(values[String.Format(names.ColumnSearchValue, counter)], out string? columnSearchValue);

                // Parses IsRegex value.
                Parse(values[String.Format(names.IsColumnSearchRegex, counter)], out bool columnSearchRegex);

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
        private static void ParseSorting(IEnumerable<Column> columns, IFormCollection values, RequestNameConvention names)
        {
            for (int i = 0; i < columns.Count(); i++)
            {
                if (!Parse(values[String.Format(names.SortField, i)], out int sortField))
                    break;

                var column = columns.ElementAt(sortField);

                if (!column.IsSortable)
                    continue;

                Parse(values[String.Format(names.SortDirection, i)], out string sortDirection);

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
        private static bool Parse<ElementType>(StringValues value, out ElementType result)
        {
            result = default!;

            if (string.IsNullOrWhiteSpace(value.FirstOrDefault()))
                return false;

            try
            {
                result = (ElementType)Convert.ChangeType(value.First()!, typeof(ElementType));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
