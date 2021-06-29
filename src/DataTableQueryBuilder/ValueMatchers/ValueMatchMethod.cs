using System;

namespace DataTableQueryBuilder.ValueMatchers
{
    public enum ValueMatchMethod
    {
        //Equals,
        Contains,
        StartsWith,
        EndsWith,
        SQLServerContainsPhrase,
        SQLServerFreeText
    }
}