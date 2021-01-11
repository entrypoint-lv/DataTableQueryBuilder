namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Represents response naming convention.
    /// </summary>
    public class ResponseNameConvention
    {
        public string Draw { get { return "draw"; } }
        public string TotalRecords { get { return "recordsTotal"; } }
        public string TotalRecordsFiltered { get { return "recordsFiltered"; } }
        public string Data { get { return "data"; } }
        public string Error { get { return "error"; } }
    }
}
