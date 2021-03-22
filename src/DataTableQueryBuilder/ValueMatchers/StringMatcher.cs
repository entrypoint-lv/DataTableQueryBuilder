using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder.ValueMatchers
{
    public class StringMatcher : ValueMatcher
    {
        protected ValueMatchMethod MatchMethod { get; set; }

        public StringMatcher(Expression property, string valueToMatch, ValueMatchMethod matchMethod) : base(property, valueToMatch)
        {
            MatchMethod = matchMethod;
        }

        public override Expression Match()
        {
            if (MatchMethod == ValueMatchMethod.SQLServerContainsPhrase || MatchMethod == ValueMatchMethod.SQLServerFreeText)
                return GenerateSQLServerFullTextSearchMatchExp();

            return GenerateStringMatchExp();
        }

        private Expression GenerateStringMatchExp()
        {
            var methodName = MatchMethod == ValueMatchMethod.StartsWith ? "StartsWith" : (MatchMethod == ValueMatchMethod.EndsWith ? "EndsWith" : "Contains");

            var propertyAsStringExp = Property.Type == typeof(string) ? (Expression)Expression.Coalesce(Property, Expression.Constant(string.Empty)) : Expression.Call(Property, Property.Type.GetMethod("ToString", Type.EmptyTypes));

            var propertyToLowerExp = Expression.Call(propertyAsStringExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            var propertyMatchExp = Expression.Call(propertyToLowerExp, typeof(string).GetMethod(methodName, new[] { typeof(string) }), Expression.Constant(ValueToMatch.ToLower()));

            return propertyMatchExp;
        }

        private Expression GenerateSQLServerFullTextSearchMatchExp()
        {
            var sqlServerMethodName = MatchMethod == ValueMatchMethod.SQLServerContainsPhrase ? "Contains" : "FreeText";
            var valueToMatch = MatchMethod == ValueMatchMethod.SQLServerContainsPhrase ? $"\"{ValueToMatch}*\"" : ValueToMatch;

            var methodInfo = typeof(SqlServerDbFunctionsExtensions).GetMethod(sqlServerMethodName, BindingFlags.Static | BindingFlags.Public, null, new[] { EF.Functions.GetType(), typeof(string), typeof(string) }, null);

            return Expression.Call(methodInfo, Expression.Constant(EF.Functions), Property, Expression.Constant(valueToMatch));
        }
    }
}