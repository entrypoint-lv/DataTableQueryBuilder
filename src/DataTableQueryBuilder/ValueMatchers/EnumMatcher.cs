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

            if (!Enum.TryParse(enumType, ValueToMatch, true, out object? val))
                return NoMatch;

            return Expression.Equal(ToNullable(Property), Expression.Constant(val, typeof(Nullable<>).MakeGenericType(Property.Type)));
        }
    }
}