using System.Linq;
using System.Collections.Generic;

namespace DataTableQueryBuilder.DataTables
{
    public class DataTablesBuildResult<TDataTableFields, TEntity> : BuildResult<TDataTableFields, TEntity>
    {
        private readonly DataTablesRequest request;

        public DataTablesBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQuery, DataTablesRequest request) : base(totalRecords, totalRecordsFiltered, buildedQuery)
        {
            this.request = request;
        }

        protected override IDataTableResponse CreateResponse(IEnumerable<TDataTableFields> model, Dictionary<string, object>? responseAdditionalParams)
        {
            return new DataTablesResponse(request, TotalRecords, TotalRecordsFiltered, model, responseAdditionalParams);
        }
    }
}