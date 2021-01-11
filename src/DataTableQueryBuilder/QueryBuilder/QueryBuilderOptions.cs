using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder
{
    public class QueryBuilderOptions<TDataTableFields, TEntity>
    {
        private readonly string DefaultDateFormat = CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern;

        private readonly IDataTableRequest request;

        protected readonly IDictionary<string, FieldOptions<TEntity>> fieldOptions;

        /// <summary>
        /// Gets or sets date format of searching values.
        /// </summary>
        public string DateFormat { get; set; }

        public QueryBuilderOptions(IDataTableRequest request) {
            this.request = request;

            fieldOptions = InitFieldOptions();

            DateFormat = DefaultDateFormat;
        }

        /// <summary>
        /// Customizes the options for individual field.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="property">Expression to the field</param>
        /// <param name="optionsAction">An action to configure the FieldOptions.</param>
        /// <returns>Query builder options.</returns>
        public void ForField<TMember>(Expression<Func<TDataTableFields, TMember>> property, Action<FieldOptions<TEntity>> optionsAction)
        {
            var fieldName = ((MemberExpression)property.Body).Member.Name;

            optionsAction(GetFieldOptions(fieldName));
        }

        /// <summary>
        /// Gets the options for individual field.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Field options.</returns>
        public FieldOptions<TEntity> GetFieldOptions(string fieldName)
        {
            var fieldNameInPascalCase = Char.ToLower(fieldName[0]) + fieldName.Substring(1);

            return fieldOptions[fieldNameInPascalCase];
        }

        /// <summary>
        /// Validates the query builder options.
        /// </summary>
        internal void Validate()
        {
            foreach (var field in request.SearchableFields)
            {
                var opt = fieldOptions[field.Key];

                if (opt == null)
                    continue;

                if (opt.SourceProperty == null && opt.SearchExpression == null)
                    throw new Exception($"Can't figure out how to search the field '{field.Key}'. Mark column as not searchable or use UseEntityProperty() or SetSearchExp() methods.");

                if (opt.SearchExpression != null)
                {
                    bool parameterIsUsed = ExpressionHelper.FindParameter(opt.SearchExpression.Body, opt.SearchExpression.Parameters[1]);

                    if (!parameterIsUsed)
                        throw new Exception($"Search value is not used in SearchExpression for field '{field.Key}'");
                }
            }

            foreach (var field in request.SortableFields)
            {
                var opt = fieldOptions[field.Key];

                if (opt != null && opt.SourceProperty == null && opt.SortExpression == null)
                    throw new Exception($"Can't figure out how to sort field '{field.Key}'. Mark column as not sortable or use UseEntityProperty() or SetSortExp() methods.");
            }
        }

        /// <summary>
        /// Initializes the field options based on the data table request.
        /// </summary>
        /// <returns>Field options.</returns>
        protected Dictionary<string, FieldOptions<TEntity>> InitFieldOptions()
        {
            var fields = request.SearchableFields.Keys.Union(request.SortableFields.Keys).Distinct();

            var fieldOptions = new Dictionary<string, FieldOptions<TEntity>>();

            foreach (var field in fields)
            {
                //Try to find TEntity's property by field name
                var prop = typeof(TEntity).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                //Create access expression to TEntity's property, if property was found
                var propertyAccessExpression = prop == null ? null : Expression.MakeMemberAccess(Expression.Parameter(typeof(TEntity), "target"), prop!);

                fieldOptions.Add(field, new FieldOptions<TEntity>(propertyAccessExpression));
            }

            return fieldOptions;
        }

    }
}