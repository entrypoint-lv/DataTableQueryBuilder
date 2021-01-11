using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class IntegerMatcher : ValueMatcher
    {
        public IntegerMatcher(Expression property, string valueToMatch) : base(property, valueToMatch) { }

        public override Expression Match()
        {
            if (!int.TryParse(ValueToMatch, out int val))
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(val, typeof(int?)));
        }
    }
}
