# .NET LINQ query builder for JavaScript datatables

With this builder, you can implement server-side paging, filtering and sorting for any JavaScript datatable with just a few lines of code.

The builder dynamically transforms AJAX requests coming from a JavaScript datatable into LINQ queries against the Entity Framework data model.

Can be used with any JavaScript datatable component that supports server-side processing.

Currently tested on [TanStack Table](https://tanstack.com/table), [datatables.net](https://datatables.net) and [vue-good-table](https://xaksis.github.io/vue-good-table/).

## General usage (TanStack Table and others)

Install the [DataTableQueryBuilder.Generic](https://www.nuget.org/packages/datatablequerybuilder.generic/) NuGet package:

```shell
dotnet add package DataTableQueryBuilder.Generic
```

In order to automatically bind incoming AJAX requests to `DataTableRequest` model, your JavaScript datatable should send requests in a [specific JSON format](request-format#datatablequerybuilder-generic-package).

## Usage with DataTables

If you're using [DataTables](https://datatables.net) or wrappers around it, install the [DataTableQueryBuilder.DataTables](https://www.nuget.org/packages/datatablequerybuilder.datatables/) NuGet package instead of `Generic` package:

```shell
dotnet add package DataTableQueryBuilder.DataTables
```

Then register the built-in model binder to automatically bind incoming AJAX requests from DataTables to `DataTableRequest` model:

```c#
using DataTableQueryBuilder.DataTables;
   
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //...           
        services.RegisterDataTables();
    }   
}
```


