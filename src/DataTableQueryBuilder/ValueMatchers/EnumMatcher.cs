using System;
using System.Linq.Expressions;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class EnumMatcher : ValueMatcher
    {
        public EnumMatcher(Expression property, string valueToMatch) : base(property, valueToMatch) { }

        public override Expression Match()
        {
            var enumType = Nullable.GetUnderlyingType(Property.Type) ?? Property.Type;

            Enum.TryParse(enumType, ValueToMatch, true, out object? val);

            if (val == null)
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(val, Property.Type));
        }
    }
}