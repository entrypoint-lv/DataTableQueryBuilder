namespace DataTableQueryBuilder
{
    /// <summary>
    /// Represents sort/ordering for a field.
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets sort direction.
        /// </summary>
        public SortDirection Direction { get; private set; }

        /// <summary>
        /// Gets sort order.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Creates a new sort instance.
        /// </summary>
        /// <param name="order">Sort order for multi-sorting.</param>
        /// <param name="direction">Sort direction.</param>
        public Sort(int order, string direction)
        {
            Order = order;

            // Default is ascending sort. Descending sort should be explicitly set.
            Direction = (direction ?? "").ToLowerInvariant().Equals("desc") ? SortDirection.Descending : SortDirection.Ascending;
        }
    }

    /// <summary>
    /// Sort direction enum.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Ascending direction.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Descending direction.
        /// </summary>
        Descending = 1
    }
}
