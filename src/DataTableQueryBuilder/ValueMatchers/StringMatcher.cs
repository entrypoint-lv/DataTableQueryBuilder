using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class StringMatcher : ValueMatcher
    {
        protected StringMatchMode MatchMode { get; private set; }

        public StringMatcher(Expression property, string valueToMatch, StringMatchMode valueMatchMode) : base(property, valueToMatch)
        {
            MatchMode = valueMatchMode;
        }

        public override Expression Match()
        {
            if (MatchMode == StringMatchMode.SQLServerContainsPhrase || MatchMode == StringMatchMode.SQLServerFreeText)
                return GenerateSQLServerFullTextSearchMatchExp();

            return GenerateStringMatchExp();
        }

        private Expression GenerateStringMatchExp()
        {
            var methodName = MatchMode == StringMatchMode.StartsWith ? "StartsWith" : (MatchMode == StringMatchMode.EndsWith ? "EndsWith" : "Contains");

            var propertyAsStringExp = Property.Type == typeof(string) ? (Expression)Expression.Coalesce(Property, Expression.Constant(string.Empty)) : Expression.Call(Property, Property.Type.GetMethod("ToString", Type.EmptyTypes)!);

            var propertyToLowerExp = Expression.Call(propertyAsStringExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

            return Expression.Call(propertyToLowerExp, typeof(string).GetMethod(methodName, new[] { typeof(string) })!, Expression.Constant(ValueToMatch.ToLower()));
        }

        private Expression GenerateSQLServerFullTextSearchMatchExp()
        {
            var sqlServerMethodName = MatchMode == StringMatchMode.SQLServerContainsPhrase ? "Contains" : "FreeText";
            var valueToMatch = MatchMode == StringMatchMode.SQLServerContainsPhrase ? $"\"{ValueToMatch}*\"" : ValueToMatch;

            var methodInfo = typeof(SqlServerDbFunctionsExtensions).GetMethod(sqlServerMethodName, BindingFlags.Static | BindingFlags.Public, null, new[] { EF.Functions.GetType(), typeof(string), typeof(string) }, null)!;

            return Expression.Call(methodInfo, Expression.Constant(EF.Functions), Property, Expression.Constant(valueToMatch));
        }
    }
}