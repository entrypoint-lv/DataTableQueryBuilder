using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class BooleanMatcher : ValueMatcher
    {
        public BooleanMatcher(Expression property, string valueToMatch) : base(property, valueToMatch) { }

        public override Expression Match()
        {
            if (!bool.TryParse(ValueToMatch, out bool val))
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(val, typeof(bool?)));
        }
    }
}
