using System.Collections.Generic;
using System.Linq;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a DataTables request.
    /// </summary>
    public class DataTablesRequest : IDataTableRequest
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
        /// A value <= 0 to indicate that all records should be returned (although that negates any benefits of server-side processing!).
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets global search.
        /// To be applied to all searchable columns.
        /// </summary>
        public Search Search { get; }

        /// <summary>
        /// Gets DataTables column collection (from client-side).
        /// </summary>
        public IEnumerable<Column> Columns { get; }

        public string? GlobalSearchValue
        {
            get
            {
                return this.Search.Value;
            }
        }

        public Dictionary<string, string> SearchableFields
        {
            get
            {
                return Columns.Where(c => c.IsSearchable && c.Search != null).ToDictionary(c => c.Field, c => c.Search!.Value);
            }
        }

        public Dictionary<string, Sort> SortableFields
        {
            get
            {
                return Columns.Where(c => c.IsSortable && c.Sort != null).ToDictionary(c => c.Field, c => c.Sort!);
            }
        }

        /// <summary>
        /// Gets the user-defined collection of parameters.
        /// </summary>
        public IDictionary<string, object>? AdditionalParameters { get; }

        public DataTablesRequest(int draw, int start, int pagesize, Search search, IEnumerable<Column> columns) : this(draw, start, pagesize, search, columns, null)
        { }

        public DataTablesRequest(int draw, int start, int pageSize, Search search, IEnumerable<Column> columns, IDictionary<string, object>? additionalParameters)
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
