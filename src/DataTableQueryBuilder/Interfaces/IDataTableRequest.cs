using System.Collections.Generic;

namespace DataTableQueryBuilder
{
    /// <summary>
    /// Represents a generic data table request.
    /// </summary>
    public interface IDataTableRequest
    {
        /// <summary>
        /// Gets paging first record indicator.
        /// This is the start point in the current data set (zero index based).
        /// </summary>
        public int StartRecordNumber { get; }

        /// <summary>
        /// Gets the number of records that the table can display in the current draw.
        /// It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the search value to be applied to all columns that have global search enabled.
        /// </summary>
        public string? SearchValue { get; }

        /// <summary>
        /// Gets searchable fields.
        /// </summary>
        public Dictionary<string, string?> SearchableFields { get; }

        /// <summary>
        /// Gets sortable value.
        /// </summary>
        public Dictionary<string, Sort?> SortableFields { get; }
    }
}
