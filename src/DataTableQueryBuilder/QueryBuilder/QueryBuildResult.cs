using System;
using System.Linq;
using System.Collections.Generic;

namespace DataTableQueryBuilder
{
    /// <summary>
    /// Represents a query build result.
    /// </summary>
    public class QueryBuildResult<TDataTableFields, TEntity>
    {
        /// <summary>
        /// Gets the total number of records without filtering applied.
        /// </summary>
        public int TotalRecords { get; private set; }

        /// <summary>
        /// Gets the total number of records with filtering applied.
        /// </summary>
        public int TotalRecordsFiltered { get; private set; }

        /// <summary>
        /// Gets the builded query without pagination with specified searching and sorting configuration applied.
        /// </summary>
        public IQueryable<TEntity> BuildedQueryWithoutPagination { get; private set; }

        /// <summary>
        /// Gets the builded query with pagination and specified searching and sorting configuration applied.
        /// </summary>
        public IQueryable<TEntity> BuildedQuery { get; private set; }

        /// <summary>
        /// Creates a new build result.
        /// </summary>
        /// <param name="totalRecords">Total records in the data source.</param>
        /// <param name="totalRecordsFiltered">Total records filtered from the data source.</param>
        /// <param name="buildedQueryWithoutPagination">Builded LINQ query without pagination.</param>
        /// <param name="buildedQuery">Builded LINQ query.</param>
        public QueryBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQueryWithoutPagination, IQueryable<TEntity> buildedQuery)
        {
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            BuildedQueryWithoutPagination = buildedQueryWithoutPagination;
            BuildedQuery = buildedQuery;
        }
    }
}