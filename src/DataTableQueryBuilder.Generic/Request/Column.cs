namespace DataTableQueryBuilder.Generic
{
    /// <summary>
    /// Represents a Generic column.
    /// </summary>
    public class Column
    {
        public string Field { get; set; }
        public string? Name { get; set; }
        public bool Searchable { get; set; } = true;
        public string Search { get; set; } = string.Empty;
        public bool Sortable { get; set; } = true;
        public string Sort { get; set; } = string.Empty;
    }
}
