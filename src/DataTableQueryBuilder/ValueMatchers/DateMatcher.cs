using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class DateMatcher : ValueMatcher
    {
        protected string DateFormat { get; private set; }

        public DateMatcher(Expression property, string valueToMatch, string dateFormat) : base(property, valueToMatch)
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

            dateTill = dateTill.Value.AddDays(1);

            var p = ToNullable(Property);

            var propertyGreatherThan = Expression.GreaterThanOrEqual(p, Expression.Constant(dateFrom, typeof(DateTime?)));
            var propertyLessThan = Expression.LessThan(p, Expression.Constant(dateTill, typeof(DateTime?)));

            return Expression.AndAlso(propertyGreatherThan, propertyLessThan);
        }

        private Expression MatchByExactDate(string date)
        {
            var dateVal = ParseDate(date);

            if (!dateVal.HasValue)
                return NoMatch;

            var propertyDate = Expression.Property(Expression.Property(ToNullable(Property), "Value"), "Date");

            return Expression.Equal(propertyDate, Expression.Constant(dateVal.Value.Date));
        }

        private DateTime? ParseDate(string date)
        {
            bool success = DateTime.TryParseExact(date.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

            return success ? (DateTime?)parsedDate : null;
        }
    }
}