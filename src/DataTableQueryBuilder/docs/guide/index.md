# LINQ query builder for JavaScript datatables that support server-side processing

This builder automatically transforms an AJAX request coming from a JavaScript datatable into a LINQ query against the Entity Framework data model according to the provided configuration.

Can be used with ANY JavaScript datatable component that supports server-side processing (currently tested on [datatables.net](https://datatables.net) and [vue-good-table](https://xaksis.github.io/vue-good-table/) only).

## Usage with DataTables

If you're using [datatables.net](https://datatables.net) or wrappers around it, install the [DataTableQueryBuilder.DataTables](https://www.nuget.org/packages/datatablequerybuilder.datatables/) NuGet package:

   ```shell
   dotnet add package DataTableQueryBuilder.DataTables
   ```

Then register the model binder to bind incoming AJAX requests from DataTables to a DataTableRequest model:

   ```c#
   using DataTableQueryBuilder.DataTables;
   
   public class Startup
   {
       //...

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

Nothing else is needed.



