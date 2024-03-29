﻿using System;
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
    public class DataTableQueryBuilder<TDataTableFields, TSource> : QueryBuilder<TDataTableFields, TSource>
    {
        /// <summary>
        /// Creates a new query builder to be used for specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        public DataTableQueryBuilder(DataTableRequest request) : base(request)
        { }

        /// <summary>
        /// Creates a new query builder to be used on specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        /// <param name="optionsAction">An action to configure the QueryBuilderOptions.</param>
        public DataTableQueryBuilder(DataTableRequest request, Action<QueryBuilderOptions<TDataTableFields, TSource>> optionsAction) : base(request, optionsAction)
        { }

        public DataTableBuildResult<TDataTableFields, TSource> Build(IQueryable<TSource> sourceQuery)
        {
            var res = BuildQuery(sourceQuery);

            return new DataTableBuildResult<TDataTableFields, TSource>(res.TotalRecords, res.TotalRecordsFiltered, res.BuildedQueryWithoutPagination, res.BuildedQuery, (DataTableRequest)request);
        }
    }

    /// <summary>
    /// Builds a query according to the specific DataTable request.
    /// </summary>
    /// <typeparam name="TSource">The type of the data returned from the data source.</typeparam>
    public class DataTableQueryBuilder<TSource> : QueryBuilder<TSource, TSource>
    {
        /// <summary>
        /// Creates a new query builder to be used for specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        public DataTableQueryBuilder(DataTableRequest request) : base(request)
        { }

        /// <summary>
        /// Creates a new query builder to be used on specific DataTable request.
        /// </summary>
        /// <param name="request">DataTables request.</param>
        /// <param name="optionsAction">An action to configure the QueryBuilderOptions.</param>
        public DataTableQueryBuilder(DataTableRequest request, Action<QueryBuilderOptions<TSource, TSource>> optionsAction) : base(request, optionsAction)
        { }

        public DataTablesBuildResult<TSource> Build(IQueryable<TSource> sourceQuery)
        {
            var res = BuildQuery(sourceQuery);

            return new DataTablesBuildResult<TSource>(res.TotalRecords, res.TotalRecordsFiltered, res.BuildedQueryWithoutPagination, res.BuildedQuery, (DataTableRequest)request);
        }
    }
}