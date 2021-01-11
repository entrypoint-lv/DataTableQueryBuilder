using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DataTableQueryBuilder.DataTables;

namespace DataTableQueryBuilder
{
    using ValueMatchers;

    /// <summary>
    /// Builds a query according to the specific data table request.
    /// </summary>
    /// <typeparam name="TDestination">A view model that represents the list of data table fields.</typeparam>
    /// <typeparam name="TSource">The type of the data returned from the data source.</typeparam>
    public abstract class QueryBuilder<TDestination, TSource>
    {
        protected readonly IDataTableRequest request;

        /// <summary>
        /// Gets the builder options.
        /// </summary>
        public QueryBuilderOptions<TDestination, TSource> Options { get; private set; }

        /// <summary>
        /// Creates a new query builder to be used for specific data table request.
        /// </summary>
        /// <param name="request">Data table request.</param>
        public QueryBuilder(IDataTableRequest request)
        {
            this.request = request;

            Options = new QueryBuilderOptions<TDestination, TSource>(request);
        }

        /// <summary>
        /// Creates a new request parser to be used on specific data table request.
        /// </summary>
        /// <param name="request">Data table request.</param>
        /// <param name="optionsAction">An action to configure the QueryBuilderOptions.</param>
        public QueryBuilder(IDataTableRequest request, Action<QueryBuilderOptions<TDestination, TSource>> optionsAction) : this(request)
        {
            optionsAction(Options);
        }

        /// <summary>
        /// Builds the query according to the specified data table request and returns the result.
        /// </summary>
        /// <param name="sourceQuery">The query against a specific data source to which the specified searching and sorting request should be applied.</param>
        /// <returns>Build result.</returns>
        public BuildResult<TDestination, TSource> Build(IQueryable<TSource> sourceQuery)
        {
            Options.Validate();

            var totalRecords = sourceQuery.Count();

            var buildedQuery = ApplySearchExpression(sourceQuery);
            buildedQuery = ApplySortExpression(buildedQuery);

            var totalRecordsFiltered = buildedQuery.Count();

            //apply pagination
            if (request.PageSize > 0)
                buildedQuery = buildedQuery.Skip(request.StartRecordNumber).Take(request.PageSize);

            return CreateBuildResult(totalRecords, totalRecordsFiltered, buildedQuery);
        }

        protected abstract BuildResult<TDestination, TSource> CreateBuildResult(int totalRecords, int totalRecordsFiltered, IQueryable<TSource> buildedQuery);

        private IQueryable<TSource> ApplySearchExpression(IQueryable<TSource> query)
        {
            ParameterExpression target = Expression.Parameter(typeof(TSource), "target");

            var fieldSearchExp = BuildFieldSearchExpression(target);
            var globalSearchExp = BuildGlobalSearchExpression(target);

            if (fieldSearchExp == null && globalSearchExp == null)
                return query;

            var searchExp = fieldSearchExp ?? globalSearchExp;

            if (fieldSearchExp != null && globalSearchExp != null)
                searchExp = Expression.AndAlso(fieldSearchExp, globalSearchExp);

            return ExpressionHelper.AddWhere(query, searchExp, target);
        }

        private Expression? BuildFieldSearchExpression(ParameterExpression target)
        {
            var filedsToSearch = request.SearchableFields.Where(f => !string.IsNullOrEmpty(f.Value));

            if (filedsToSearch.Count() == 0)
                return null;

            Expression? exp = null;

            foreach (var field in filedsToSearch)
            {
                var opt = Options.GetFieldOptions(field.Key);

                if (opt == null)
                    continue;

                Expression? matchExp = null;

                if (opt.SearchExpression != null)
                {
                    //replace expression parameters
                    matchExp = ExpressionHelper.Replace(opt.SearchExpression.Body, opt.SearchExpression.Parameters[0], target);
                    matchExp = ExpressionHelper.Replace(matchExp, opt.SearchExpression.Parameters[1], Expression.Constant(field.Value));
                }
                else
                {
                    matchExp = BuildMatchExpression(opt.SourceProperty, field.Value, opt.ValueMatchMethod, target);
                }

                if (matchExp != null)
                    exp = (exp == null) ? matchExp : Expression.AndAlso(exp, matchExp);
            }

            return exp;
        }

        private Expression? BuildGlobalSearchExpression(ParameterExpression target)
        {
            if (string.IsNullOrEmpty(request.GlobalSearchValue))
                return null;

            Expression? exp = null;

            foreach (var field in request.SearchableFields)
            {
                var opt = Options.GetFieldOptions(field.Key);

                if (opt == null)
                    continue;

                Expression? matchExp = null;

                if (!opt.IsGlobalSearchEnabled)
                {
                    matchExp = Expression.Constant(false);
                }
                else if (opt.SearchExpression != null)
                {
                    //replace expression parameters
                    matchExp = ExpressionHelper.Replace(opt.SearchExpression.Body, opt.SearchExpression.Parameters[0], target);
                    matchExp = ExpressionHelper.Replace(matchExp, opt.SearchExpression.Parameters[1], Expression.Constant(request.GlobalSearchValue));
                }
                else
                {
                    matchExp = BuildMatchExpression(opt.SourceProperty, request.GlobalSearchValue, opt.ValueMatchMethod, target);
                }

                if (matchExp != null)
                    exp = (exp == null) ? matchExp : Expression.OrElse(exp, matchExp);
            }

            return exp;
        }

        private Expression? BuildMatchExpression(Expression? targetProperty, string propertyValue, ValueMatchMethod valueMatchMethod, ParameterExpression target)
        {
            if (targetProperty == null)
                return null;

            if (string.IsNullOrEmpty(propertyValue))
                return null;

            var propertyExp = ExpressionHelper.ExtractPropertyChain(targetProperty, target);

            return ValueMatcher.Create(propertyExp, propertyValue, valueMatchMethod, Options.DateFormat).Match();
        }

        private IQueryable<TSource> ApplySortExpression(IQueryable<TSource> query)
        {
            var fieldsToSort = request.SortableFields.Where(f => f.Value != null).OrderBy(f => f.Value.Order);

            if (fieldsToSort.Count() == 0)
                return query;

            var q = query;

            ParameterExpression target = Expression.Parameter(typeof(TSource), "target");

            bool orderByAdded = false;

            foreach (var field in fieldsToSort)
            {
                var opt = Options.GetFieldOptions(field.Key);

                if (opt == null)
                    continue;

                Expression? sortExp = null;

                if (opt.SortExpression != null)
                {
                    //replace expression parameters
                    sortExp = ExpressionHelper.Replace(opt.SortExpression.Body, opt.SortExpression.Parameters[0], target);
                }
                else if (opt.SourceProperty != null)
                {
                    sortExp = ExpressionHelper.ExtractPropertyChain(opt.SourceProperty, target);
                }

                if (sortExp == null)
                    continue;

                if (!orderByAdded)
                {
                    q = (field.Value.Direction == SortDirection.Ascending) ? ExpressionHelper.AddOrderBy(q, sortExp, target) : ExpressionHelper.AddOrderByDescending(q, sortExp, target);
                    orderByAdded = true;
                }
                else
                {
                    q = (field.Value.Direction == SortDirection.Ascending) ? ExpressionHelper.AddThenBy(q, sortExp, target) : ExpressionHelper.AddThenByDescending(q, sortExp, target);
                }
            }

            return q;
        }

    }
}