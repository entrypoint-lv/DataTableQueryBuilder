using System.Collections.Generic;
using System.Linq;

namespace DataTableQueryBuilder.Generic
{
    /// <summary>
    /// Represents a Generic request.
    /// </summary>
    public class DataTableRequest : IDataTableRequest
    {
        /// <summary>
        /// Current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets the number of records that the table can display in the current draw.
        /// It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
        /// A value &lt;= 0 to indicate that all records should be returned (although that negates any benefits of server-side processing!).
        /// </summary>
        public int PageSize { get; set; } = Options.DefaultPageLength;


        /// <summary>
        /// Gets paging first record indicator.
        /// This is the start point in the current data set (zero index based).
        /// </summary>
        public int StartRecordNumber => (Page - 1) * PageSize;

        /// <summary>
        /// Gets the search value to be applied to all columns that have global search enabled.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Gets the search value to be applied to all columns that have global search enabled.
        /// </summary>
        public string? SearchValue => Search;

        /// <summary>
        /// Gets Generic column collection (from client-side).
        /// </summary>
        public List<Column> Columns { get; set; } = new List<Column>();

        /// <summary>
        /// Gets searchable fields.
        /// </summary>
        public Dictionary<string, string> SearchableFields => Columns.Where(c => c.Searchable).ToDictionary(c => c.Field, c => c.Search);

        /// <summary>
        /// Gets sortable fields.
        /// </summary>
        public Dictionary<string, Sort> SortableFields
        {
            get
            {
                Dictionary<string, Sort> sortableFields = new Dictionary<string, Sort>();

                foreach (Column column in Columns.Where(c => c.Sortable && !string.IsNullOrEmpty(c.Sort)))
                    sortableFields.Add(column.Field, new Sort(sortableFields.Count, column.Sort));

                return sortableFields;
            }
        }
    }
}
