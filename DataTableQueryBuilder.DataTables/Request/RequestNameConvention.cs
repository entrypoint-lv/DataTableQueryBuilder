namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents request naming convention.
    /// </summary>
    public class RequestNameConvention
    {
        public string Draw { get { return "draw"; } }
        public string StartRecordNumber { get { return "start"; } }
        public string PageSize { get { return "length"; } }
        public string SearchValue { get { return "search[value]"; } }
        public string IsSearchRegex { get { return "search[regex]"; } }
        public string SortField { get { return "order[{0}][column]"; } }
        public string SortDirection { get { return "order[{0}][dir]"; } }
        public string ColumnField { get { return "columns[{0}][data]"; } }
        public string ColumnName { get { return "columns[{0}][name]"; } }
        public string IsColumnSearchable { get { return "columns[{0}][searchable]"; } }
        public string IsColumnSortable { get { return "columns[{0}][orderable]"; } }
        public string ColumnSearchValue { get { return "columns[{0}][search][value]"; } }
        public string IsColumnSearchRegex { get { return "columns[{0}][search][regex]"; } }
        public string SortAscending { get { return "asc"; } }
        public string SortDescending { get { return "desc"; } }
    }
}
