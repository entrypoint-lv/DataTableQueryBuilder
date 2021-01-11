using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder.DataTables
{
    /// <summary>
    /// Builds a query according to the specific DataTable request.
    /// </summary>
    /// <typeparam name="TDataTableFields">A view model that represents the list of DataTable columns.</typeparam>
    /// <typeparam name="TSource">The type of the data returned from the data source.</typeparam>
    public class DataTablesQueryBuilder<TDataTableFields, TSource> : QueryBuilder<TDataTableFields, TSource>
    {
        /// <summary>
        /// Creates a new query builder to be used for specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        public DataTablesQueryBuilder(DataTablesRequest request) : base(request)
        { }

        /// <summary>
        /// Creates a new query builder to be used on specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        /// <param name="optionsAction">An action to configure the QueryBuilderOptions.</param>
        public DataTablesQueryBuilder(DataTablesRequest request, Action<QueryBuilderOptions<TDataTableFields, TSource>> optionsAction) : base(request, optionsAction)
        { }

        protected override BuildResult<TDataTableFields, TSource> CreateBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TSource> buildedQuery)
        {
            return new DataTablesBuildResult<TDataTableFields, TSource>(totalRecords, totalRecordsFiltered, buildedQuery, (DataTablesRequest)request);
        }
    }
}