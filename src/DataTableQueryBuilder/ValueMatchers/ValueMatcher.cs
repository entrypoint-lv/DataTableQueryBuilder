using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder.ValueMatchers
{
    public abstract class ValueMatcher
    {
        protected readonly Expression NoMatch = Expression.Constant(false);

        protected Expression Property { get; set; }
        protected string ValueToMatch { get; set; }

        protected ValueMatcher(Expression property, string valueToMatch)
        {
            Property = property;
            ValueToMatch = valueToMatch;
        }

        protected Expression ToNullable(Expression property)
        {
            var isNullable = Nullable.GetUnderlyingType(Property.Type) != null;

            return isNullable ? property : Expression.Convert(property, typeof(Nullable<>).MakeGenericType(Property.Type));
        }

        public abstract Expression Match();
    }
}
