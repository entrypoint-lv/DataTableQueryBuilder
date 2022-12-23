# Server-side .NET query builder for JavaScript data tables

This builder automatically transforms AJAX requests coming from a JavaScript datatable into LINQ queries against the Entity Framework data model according to the provided configuration.

Can be used with any JavaScript datatable component that supports server-side processing (currently tested on [datatables.net](https://datatables.net) and [vue-good-table](https://xaksis.github.io/vue-good-table/) only).

## Usage with DataTables

If you're using [DataTables](https://datatables.net) or wrappers around it, install the [DataTableQueryBuilder.DataTables](https://www.nuget.org/packages/datatablequerybuilder.datatables/) NuGet package:

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

 ## Usage with other JavaScript datatables

For other JavaScript datatable components, install the [DataTableQueryBuilder.Generic](https://www.nuget.org/packages/datatablequerybuilder.generic/) NuGet package instead:

```shell
dotnet add package DataTableQueryBuilder.Generic
```

In order to automatically bind incoming AJAX requests to `DataTableRequest` model, your JavaScript datatable should send requests in a [specific JSON format](request-format#datatablequerybuilder-generic-package).



