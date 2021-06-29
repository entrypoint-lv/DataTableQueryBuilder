using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder
{
    public static class TypeHelper
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

        public static bool IsInteger(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if(underlyingType == null)
                return false;

            return integerTypes.Contains(type) || integerTypes.Contains(underlyingType);
        }

        public static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || Nullable.GetUnderlyingType(type) == typeof(bool);
        }

        public static bool IsDateTime(Type type)
        {
            return type == typeof(DateTime) || Nullable.GetUnderlyingType(type) == typeof(DateTime);
        }

        public static bool IsEnum(Type type)
        {
            if (type.GetTypeInfo().IsEnum)
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType == null)
                return false;

            return underlyingType.GetTypeInfo().IsEnum;
        }
    }
}
