using System.Collections.Generic;
using System.Linq;

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
    }
}
