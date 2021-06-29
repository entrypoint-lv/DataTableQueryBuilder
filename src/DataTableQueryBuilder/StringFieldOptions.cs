using System;
using System.Linq.Expressions;

namespace DataTableQueryBuilder
{
    using ValueMatchers;

    public class StringFieldOptions<T> : FieldOptions<T>
    {
        /// <summary>
        /// Gets the value match method that is used when searching. Default is String.Contains() for strings and integers, and equals for other value types.
        /// </summary>
        internal ValueMatchMethod ValueMatchMethod { get; private set; }

        public StringFieldOptions(Expression? sourcePropertyAccessExp) : base(sourcePropertyAccessExp)
        {
            ValueMatchMethod = ValueMatchMethod.Contains;
        }

        /// <summary>
        /// Explicitly sets the value match method to be used during searching.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void UseValueMatchMethod(ValueMatchMethod method)
        {
            ValueMatchMethod = method;
        }
    }
}
