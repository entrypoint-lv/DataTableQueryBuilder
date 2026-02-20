# Request Format

## DataTableQueryBuilder.Generic package

In order to automatically bind incoming requests to `DataTableRequest` model, your JavaScript datatable should send requests in the following JSON format:

```js
columns: [
    { field: 'fullName', search: 'John' },
    { field: 'companyName', search: 'Goo' },
    { field: 'posts', search: '5', sort: 'asc' },
    { field: 'createDate', search: '05/15/2020', sort: 'desc' }
],
page: 1,
pazeSize: 20,
search: ''
```

Here:

- `fullName`, `companyName`, `posts` and `createDate` are fields in a JSON array returned by the server.

- `search` is a global (multi-column) search value.

- `sort` value should be either `asc` or `desc`. Multi-column sorting is supported.

In case you want to use a different format, you can write a custom [ModelBinder](https://github.com/entrypoint-lv/DataTableQueryBuilder/tree/master/src/DataTableQueryBuilder.DataTables/ModelBinder) that will bind incoming requests to [DataTableRequest](https://github.com/entrypoint-lv/DataTableQueryBuilder/blob/master/src/DataTableQueryBuilder.Generic/Request/DataTableRequest.cs) model that is used by the builder.

## DataTableQueryBuilder.DataTables package

Use built-in model binder to automatically bind incoming requests to `DataTableRequest` model:

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
