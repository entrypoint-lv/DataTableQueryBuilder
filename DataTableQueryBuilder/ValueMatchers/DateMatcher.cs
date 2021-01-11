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
        private readonly string dateFormat;

        public DateMatcher(Expression property, string valueToMatch, string dateFormat) : base(property, valueToMatch)
        {
            this.dateFormat = dateFormat;
        }

        public override Expression Match()
        {
            var range = ValueToMatch.Split('-');

            return range.Length > 1 ? MatchByDateRange(range) : MatchBySingleDate(range[0]);
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

        private Expression MatchBySingleDate(string date)
        {
            var dateVal = ParseDate(date);

            if (!dateVal.HasValue)
                return NoMatch;

            var propertyDate = Expression.Property(Expression.Property(ToNullable(Property), "Value"), "Date");

            return Expression.Equal(propertyDate, Expression.Constant(dateVal.Value.Date));
        }

        private DateTime? ParseDate(string date)
        {
            bool success = DateTime.TryParseExact(date.Trim(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

            return success ? (DateTime?)parsedDate : null;
        }
    }
}