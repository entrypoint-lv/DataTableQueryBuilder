using System;

namespace DataTableQueryBuilder.ValueMatchers
{
    public enum ValueMatchMethod
    {
        Equals,
        StringContains,
        StringStartsWith,
        StringEndsWith,
        StringSQLServerContainsPhrase,
        StringSQLServerFreeText
    }
}