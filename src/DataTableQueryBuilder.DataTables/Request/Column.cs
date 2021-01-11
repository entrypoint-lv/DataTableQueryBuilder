namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents a DataTables column.
    /// </summary>
    public class Column
    {
        public string Field { get; private set; }
        public string? Name { get; private set; }
        public Search? Search { get; private set; }
        public bool IsSearchable { get; private set; }
        public Sort? Sort { get; private set; }
        public bool IsSortable { get; private set; }

        public Column(string? name, string field, bool searchable, bool sortable, Search? search)
        {
            Name = name;
            Field = field;

            IsSortable = sortable;                        
            IsSearchable = searchable;

            Search = !IsSearchable ? null : search ?? new Search();
        }

        public void SetSort(int order, string direction)
        {
            if (!IsSortable)
                return;

            Sort = new Sort(order, direction);
        }
    }
}
