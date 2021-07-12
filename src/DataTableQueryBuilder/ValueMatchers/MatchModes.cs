using System;

namespace DataTableQueryBuilder.ValueMatchers
{
    public enum StringMatchMode
    {
        Contains,
        StartsWith,
        EndsWith,
        SQLServerContainsPhrase,
        SQLServerFreeText
    }

    public enum IntegerMatchMode
    {
        Equals,
        Contains
    }
}