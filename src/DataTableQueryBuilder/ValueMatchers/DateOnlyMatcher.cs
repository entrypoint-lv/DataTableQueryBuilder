using System;
using System.Globalization;
using System.Linq.Expressions;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class DateOnlyMatcher : ValueMatcher
    {
        protected string DateFormat { get; private set; }

        public DateOnlyMatcher(Expression property, string valueToMatch, string dateFormat) : base(property, valueToMatch)
        {
            DateFormat = dateFormat;
        }

        public override Expression Match()
        {
            var dateRange = ValueToMatch.Split('-');

            if (dateRange.Length == 1)
                return MatchByExactDate(dateRange[0]);

            if (dateRange.Length == 2)
                return MatchByDateRange(dateRange);

            return NoMatch;
        }

        private Expression MatchByDateRange(string[] dateRange)
        {
            var dateFrom = ParseDate(dateRange[0]);
            var dateTill = ParseDate(dateRange[1]);

            if (!dateFrom.HasValue || !dateTill.HasValue)
                return NoMatch;

            var p = ToNullable(Property);

            var propertyGreaterThan = Expression.GreaterThanOrEqual(p, Expression.Constant(dateFrom, typeof(DateOnly?)));
            var propertyLessThan = Expression.LessThanOrEqual(p, Expression.Constant(dateTill, typeof(DateOnly?)));

            return Expression.AndAlso(propertyGreaterThan, propertyLessThan);
        }

        private Expression MatchByExactDate(string date)
        {
            var dateVal = ParseDate(date);

            if (!dateVal.HasValue)
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(dateVal, typeof(DateOnly?)));
        }

        private DateOnly? ParseDate(string date)
        {
            bool success = DateOnly.TryParseExact(date.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDate);

            return success ? parsedDate : null;
        }
    }
}
