using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataTableQueryBuilder
{
    public static class ExpressionHelper
    {
        private class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;

            public ReplaceVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }

            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }

        private class ParameterFinder : ExpressionVisitor
        {
            private readonly ParameterExpression parameterToFound;

            public bool Found { get; set; }

            public ParameterFinder(ParameterExpression expression)
            {
                parameterToFound = expression;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == parameterToFound)
                    Found = true;

                return base.VisitParameter(node);
            }
        }

        public static IQueryable<T> AddWhere<T>(IQueryable<T> query, Expression? expression, ParameterExpression target)
        {
            var whereExp = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { query.ElementType },
                query.Expression,
                Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { target }));

            return query.Provider.CreateQuery<T>(whereExp);
        }

        public static IQueryable<T> AddOrderBy<T>(IQueryable<T> query, Expression expression, ParameterExpression target)
        {
            var orderBy = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { typeof(T), expression.Type },
                query.Expression,
                Expression.Lambda(expression, new ParameterExpression[] { target }));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static IQueryable<T> AddOrderByDescending<T>(IQueryable<T> query, Expression expression, ParameterExpression target)
        {
            var orderBy = Expression.Call(
                typeof(Queryable),
                "OrderByDescending",
                new Type[] { typeof(T), expression.Type },
                query.Expression,
                Expression.Lambda(expression, new ParameterExpression[] { target }));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static IQueryable<T> AddThenBy<T>(IQueryable<T> query, Expression expression, ParameterExpression target)
        {
            var orderBy = Expression.Call(
                typeof(Queryable),
                "ThenBy",
                new Type[] { typeof(T), expression.Type },
                query.Expression,
                Expression.Lambda(expression, new ParameterExpression[] { target }));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static IQueryable<T> AddThenByDescending<T>(IQueryable<T> query, Expression expression, ParameterExpression target)
        {
            var orderBy = Expression.Call(
                typeof(Queryable),
                "ThenByDescending",
                new Type[] { typeof(T), expression.Type },
                query.Expression,
                Expression.Lambda(expression, new ParameterExpression[] { target }));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static Expression Replace(Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }

        public static bool FindParameter(Expression expression, ParameterExpression parameter)
        {
            var finder = new ParameterFinder(parameter);

            finder.Visit(expression);

            return finder.Found;
        }

        public static MemberExpression ExtractPropertyChain(Expression targetProperty, ParameterExpression target)
        {
            var propertyChainExp = ExtractPropertyInfoChain(targetProperty);

            var propertyExp = Expression.Property(target, propertyChainExp.First());

            if (propertyChainExp.Count() == 1)
                return propertyExp;

            foreach (var prop in propertyChainExp.Skip(1))
            {
                propertyExp = Expression.Property(propertyExp, prop);
            }

            return propertyExp;
        }

        private static IEnumerable<PropertyInfo> ExtractPropertyInfoChain(Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression == null)
                yield break;

            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new Exception("Expression is not a property accessor");
            }

            foreach (var propertyInfo in ExtractPropertyInfoChain(memberExpression.Expression))
            {
                yield return propertyInfo;
            }

            yield return property;
        }
    }
}
