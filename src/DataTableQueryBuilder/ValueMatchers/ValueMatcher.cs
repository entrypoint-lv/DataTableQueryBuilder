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

        public static ValueMatcher Create(string fieldKey, Expression property, string valueToMatch, Enum? valueMatchMode, string dateFormat)
        {
            var type = property.Type;

            if (TypeHelper.IsInteger(type))
            {
                var integerMatchMode = IntegerMatchMode.Equals;

                if (valueMatchMode != null)
                {
                    if (!(valueMatchMode is IntegerMatchMode))
                        throw new Exception($"The ValueMatchMode for field '{fieldKey}' should be of type IntegerMatchMode.");

                    integerMatchMode = (IntegerMatchMode)valueMatchMode;
                }

                return new IntegerMatcher(property, valueToMatch, integerMatchMode);
            }

            if (TypeHelper.IsBoolean(type))
                return new BooleanMatcher(property, valueToMatch);

            if (TypeHelper.IsEnum(type))
                return new EnumMatcher(property, valueToMatch);

            if (TypeHelper.IsDateTime(type))
                return new DateMatcher(property, valueToMatch, dateFormat);

            var stringMatchMode = StringMatchMode.Contains;

            if (valueMatchMode != null)
            {
                if (!(valueMatchMode is StringMatchMode))
                    throw new Exception($"The ValueMatchMode for field '{fieldKey}' should be of type StringMatchMode.");

                stringMatchMode = (StringMatchMode)valueMatchMode;
            }

            return new StringMatcher(property, valueToMatch, stringMatchMode);
        }

        public abstract Expression Match();
    }
}
