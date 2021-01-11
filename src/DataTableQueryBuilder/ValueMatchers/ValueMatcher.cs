using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder.ValueMatchers
{
    public abstract class ValueMatcher
    {
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/integral-types-table
        private static readonly HashSet<Type> integerTypes = new HashSet<Type>
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong)
        };

        private static bool IsInteger(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if(underlyingType == null)
                return false;

            return integerTypes.Contains(type) || integerTypes.Contains(underlyingType);
        }

        private static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || Nullable.GetUnderlyingType(type) == typeof(bool);
        }

        private static bool IsDateTime(Type type)
        {
            return type == typeof(DateTime) || Nullable.GetUnderlyingType(type) == typeof(DateTime);
        }

        private static bool IsEnum(Type type)
        {
            if (type.GetTypeInfo().IsEnum)
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType == null)
                return false;

            return underlyingType.GetTypeInfo().IsEnum;
        }

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

        public static ValueMatcher Create(Expression property, string valueToMatch, ValueMatchMethod valueMatchMethod, string dateFormat)
        {
            var type = property.Type;

            if (IsInteger(type))
            {
                if (valueMatchMethod == ValueMatchMethod.Equals)
                    return new IntegerMatcher(property, valueToMatch);

                return new StringMatcher(property, valueToMatch, valueMatchMethod);
            }

            if (IsBoolean(type))
                return new BooleanMatcher(property, valueToMatch);

            if (IsEnum(type))
                return new EnumMatcher(property, valueToMatch);

            if (IsDateTime(type))
                return new DateMatcher(property, valueToMatch, dateFormat);

            return new StringMatcher(property, valueToMatch, valueMatchMethod);
        }

        public abstract Expression Match();
    }
}
