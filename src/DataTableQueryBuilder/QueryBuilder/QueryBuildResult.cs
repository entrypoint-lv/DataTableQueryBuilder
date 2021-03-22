using System;
using System.Linq;
using System.Collections.Generic;

namespace DataTableQueryBuilder
{
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
        /// Gets the builded query with specified searching and sorting configuration applied.
        /// </summary>
        public IQueryable<TEntity> BuildedQuery { get; private set; }

        public QueryBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TEntity> buildedQuery)
        {
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            BuildedQuery = buildedQuery;
        }
    }
}