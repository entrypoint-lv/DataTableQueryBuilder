using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class IntegerMatcher : ValueMatcher
    {
        protected IntegerMatchMode MatchMode { get; set; }

        public IntegerMatcher(Expression property, string valueToMatch, IntegerMatchMode valueMatchMode) : base(property, valueToMatch)
        {
            MatchMode = valueMatchMode;
        }

        public override Expression Match()
        {
            if (MatchMode == IntegerMatchMode.Equals)
                return MatchByExactValue();

            return MatchByContains();
        }

        private Expression MatchByExactValue()
        {
            if (!int.TryParse(ValueToMatch, out int val))
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(val, typeof(int?)));
        }

        private Expression MatchByContains()
        {
            var propertyAsStringExp = Expression.Call(Property, Property.Type.GetMethod("ToString", Type.EmptyTypes)!);

            var propertyToLowerExp = Expression.Call(propertyAsStringExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

            return Expression.Call(propertyToLowerExp, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, Expression.Constant(ValueToMatch.ToLower()));
        }
    }
}
