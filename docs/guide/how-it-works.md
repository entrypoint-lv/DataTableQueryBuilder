# How It Works

When user does paging, filtering or sorting, the JavaScript datatable sends a request to the server with paging, filtering and ordering clauses.

In general, this request looks something like this:

```js
columns: [
    { field: 'fullName', search: 'John' },
    { field: 'companyName', search: 'Goo' },
    { field: 'posts', search: '5', sort: 'asc' },
    { field: 'createDate', search: '05/15/2020' }
],
page: 1,
pazeSize: 20
```

::: tip Note
Here, `fullName`, `companyName`, `posts` and `createDate` are fields in a JSON array returned by the server.
:::

The task of query builder is to dynamically extend a base LINQ query with an additional `Where`, `OrderBy`, `Skip` and `Take` clauses based on this request by using expression trees.

## Default conventions

Let's review a zero configuration:

```c#
public IActionResult UserList(DataTableRequest request)
{
    // returns IQueryable<UserListData>
    var users = userService.GetAllForUserList();

    var qb = new DataTableQueryBuilder<UserListData>(request);

    var result = qb.Build(users);

    return result.CreateResponse();
}
```

As there is no any additional configuration provided, the builder will:
   
   1. Find a match between fields in the request and properties of the source `UserListData` projection model by matching their names (ignoring the case sensitivity).

   2. For each filtering clause in the request, determine the [value matching mode](value-matching) based on the type of the property in the source `UserListData` model.

As the result, with the above request and configuration, the base LINQ query will be extended in the following way:

```c#
return dataContext.Users
    //your base query
    .Select(u => new UserListData()
    {
        //...
    })
    //generated part
    .Where(p => p.FullName.ToLower().Contains("John".ToLower()))
    .Where(p => p.CompanyName.ToLower().Contains("Goo".ToLower()))
    .Where(p => p.Posts == 5)
    .Where(p => p.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
    .OrderBy(p => p.Posts)
    .Skip(0).Take(20);
```

## Additional configuration

It is possible to configure the behaviour of query builder by providing an additional options for each field:

```c#
var qb = new DataTableQueryBuilder<UserListData>(request, o =>
{
    //date format to be used when filtering dates
    o.DateFormat = "MM/dd/yyyy";
    o.ForField(f => f.FullName, o => o.UseValueMatchMode(StringMatchMode.EndsWith));
    o.ForField(f => f.CompanyName, o =>
    {
        o.UseValueMatchMode(StringMatchMode.StartsWith);
        //other options
    });
    o.ForField(f => f.Posts, o => o.UseValueMatchMode(IntegerMatchMode.Contains));
});
```

With above request and this configuration, the base LINQ query will be extended in the following way:

```c#
return dataContext.Users
    //your base query
    .Select(u => new UserListData()
    {
        //...
    })
    //generated part
    .Where(p => p.FullName.ToLower().EndsWith("John".ToLower()))
    .Where(p => p.CompanyName.ToLower().StartsWith("Goo".ToLower()))
    .Where(p => p.Posts.ToString().ToLower().Contains("5".ToLower()))
    .Where(p => p.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
    .OrderBy(p => p.Posts)
    .Skip(0).Take(20);
```

For a complete list of configuration options, see the [Configuration section](configuration/builder-options).