# LINQ query builder for any JavaScript datatable

A LINQ query builder that can be used for any JavaScript datatable or grid component that supports server-side processing.

The builder automatically transforms an AJAX request coming from JS datatable into a LINQ query against the Entity Framework data model according to the specified configuration.

## Install

If you're using https://datatables.net, install the following NuGet package:

   ```console
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

For other JS datatable components, install the generic NuGet package instead:

```console
dotnet add package DataTableQueryBuilder.Generic
```

## Samples

[Vue.js (vue-good-table) sample](https://codesandbox.io/s/vue-good-table-with-datatablequerybuilder-cynse)

[DataTables.net sample](https://codesandbox.io/s/datatablesnet-with-datatablequerybuilder-hgpg2)

Server-side API sample - [source code](https://github.com/EntryPointDev/DataTableQueryBuilder/tree/master/samples/SampleAPI) and [specification](https://query-builder-sample-api.entrypointdev.com/swagger/)

## Basic usage

Let's assume that you want to show a list of users.

In most cases, your JS datatable's configurations will look something like this:

```html
<my-datatable-component :data-url="/api/userlist" :columns="columns"></my-datatable-component>

<script>
    let columns = [
        { title: 'Id', field: 'id' },
        { title: 'Full Name', field: 'fullName' },
        { title: 'Email', field: 'email' },
        { title: 'Company Name', field: 'companyName' },
        { title: 'Posts', field: 'posts' },
        { title: 'Create Date', field: 'createDate' }
    ];
</script>
```

Your JS datatable expects server to return data for each page as a JSON array. So, a field value in each column's configuration represents a property name of a row object in this JSON array, for example:

```js
[
    { 'id': 1, 'fullName': 'John Smith', 'email': 'john@example.com', 'companyName': '', 'posts' : 0, createDate: '2021-01-05T19:38:23.551Z' }
    { 'id': 2, 'fullName': 'Michael Smith', 'email': 'michael@example.com', 'companyName': 'Apple', 'posts' : 5, createDate: '2021-04-23T18:15:43.511Z' }
    { 'id': 3, 'fullName': 'Mary Smith', 'email': 'mary@example.com', 'companyName': 'Google', 'posts' : 10, createDate: '2020-09-12T10:11:45.712Z' }
]
```

### Server-side configuration

Create a LINQ projection model that represents the fields expected by your JS datatable and returned by server:

```c#
public class UserListData
{
    public int Id { get; set; }        
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int Posts { get; set; }
    public DateTime CreateDate { get; set; }
}
```

> Please note that the `CreateDate` property is of type DateTime, so it will be returned in ISO date format - this is intentional, as the formatting must happen in the UI and not in the LINQ query.

Create a base LINQ query that will be used by query builder to request users from a database and return the required fields:

```c#
public class UserService
{
    public IQueryable<UserListData> GetAllForUserList()
    {
        return dataContext.Users
        .Select(u => new UserListData()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            CompanyName = u.Company != null ? u.Company.Name : string.Empty,
            Posts = u.Posts.Count(),
            CreateDate = u.CreateDate
        });
    }   
}
```

Create an action that will receive an AJAX request from your JS datatable, transform it to a LINQ query and return the data:

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

Thats all!

> The Build method returns a BuildResult object that contains a builded query and some other properties, expected by JS datatable - this method doesn't execute the query.

> To execute the query and return the data to the datatable, call the CreateResponse method.


For reference, the following Entity Framework data model is used in all examples:

<details>
  <summary>Entity Framework data model</summary>
  
   ```c#
   public class User
   {
       public int Id { get; set; }
       public string FullName { get; set; } = "";
       public string Email { get; set; } = "";

       public int? CompanyId { get; set; }
       public Company? Company { get; set; }

       public virtual ICollection<Post> Posts { get; } = new List<Post>();
   }

   public class Company
   {
       public int Id { get; set; }
       public string Name { get; set; } = "";

       public ICollection<User> Users { get; set; } = new List<User>();
   }

   public class Post
   {
       public int Id { get; set; }
       public string Title { get; set; } = "";
       public string Content { get; set; } = "";

       public int UserId { get; set; }
       public User User { get; set; }
   }
   ```
</details>

## How it works

When user applies filtering or sorting to some columns, the JS datatable sends a request to the server that includes a filtering and ordering clauses.

This request could look something like this:

```js
search: [{'fullName' : 'John'}, {'companyName': 'Goo'}, {'createDate': '05/15/2020'}]
sort: [{'posts' : 'asc'}]
```
> Here, `fullName`, `companyName`, `createDate` and `posts` are field names from JS datatable's configuration.

The task of the query builder is to extend a base LINQ query with an additional `Where` and `OrderBy` clauses based on this request by using expression trees.
   
If no configuration is provided, the builder will:
   
   1. Find the match between fields and target properties of the LINQ projection model by matching their names (ignoring the case sensitivity).
   2. Determine the value matching strategy to use for data filtering based on the matched target property's data type.

As the result, the base LINQ query will be extended in the following way:

```c#
//IQueryable<UserListData> users = userService.GetAllForUserList();

return dataContext.Users
    .Select(u => new UserListData()
    {
        //...
    })
    .Where(p => p.FullName.ToLower().Contains("John".ToLower()))
    .Where(p => p.CompanyName.ToLower().Contains("Goo".ToLower()))
    .Where(p => p.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
    .OrderBy(p => p.Posts);
```

## Configuration options

Built-in value matching strategies:

| Source's property type | Available matching strategies |
| :--- | :--- |
| Integral numeric types (sbyte, byte, short, ushort, int, uint, long, ulong) | Equals<br />Contains (default)
| Boolean | Equals |
| Enum | Equals |
| DateTime | The matching strategy is determined by the passed filtering value. If single date is passed - Equals, if date range is passed - Between. |
| String | Contains (default)<br />StartsWith<br />EndsWith<br />SQLServerContains<br />SQLServerFreeText |
| Any other type | Treated as string by executing `Property.ToString()` |

Builder's options:

| Property / Method | Comment | Type | Default |
| :--- | :--- | :--- | :--- |
| DateFormat | Gets or sets date format used for value matching when filtering DateTime fields. | string | CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern |
| ForField | Customizes the options for individual field. | - | - |

Individual field's options:

| Method | Comment | Arguments |
| :--- | :--- | :--- |
| UseValueMatchMode<TEnum> | Explicitly sets the value matching strategy to be used when filtering. Applicable only to properties of type `String` and Integral numeric types. | Enum of type StringMatchMode or IntegerMatchMode |
| UseSourceProperty<TMember> | Explicitly sets the property to be used when filtering and sorting. | Expression<Func<T, TMember>> property |
| SearchBy | Explicitly sets the search expression to be used when filtering. | Expression<Func<T, string, bool>> |
| OrderBy | Explicitly sets the sort expression to be used when sorting. | Expression<Func<T, object>> expression |
| EnableGlobalSearch | Enables a global search on this field. Applicable only to JS datatables supporting global search option. | - |

Example:

```c#
var qb = new DataTableQueryBuilder<UserListData>(request, o =>
{
    o.DateFormat = "MM/dd/yyyy";
    o.ForField(f => f.FullName, o => o.UseValueMatchMode(StringMatchMode.EndsWith));
    o.ForField(f => f.CompanyName, o =>
    {
        o.UseValueMatchMode(StringMatchMode.StartsWith);
        o.EnableGlobalSearch();
    });
});
```

With this configuration the base LINQ query will be extended in the following way:

```c#
//IQueryable<UserListData> users = userService.GetAllForUserList();

return dataContext.Users
    .Select(u => new UserListData()
    {
        //...
    })
    .Where(p => p.FullName.ToLower().EndsWith("John".ToLower()))
    .Where(p => p.CompanyName.ToLower().StartsWith("Goo".ToLower()))
    .Where(p => p.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
    .OrderBy(p => p.Posts);
```

## Custom search and sort expressions

Sometimes you may want to filter data in a column based on a value that doesn't belong to that column.

A common example would be the `CompanyName` column, that needs to be filtered by `companyId` value from `<select>` element that exists on the same page.

In this case, you may add the `CompanyId` property to the projection model and then use the `SearchBy` method to specify a custom LINQ expression that should be used when filtering by this field:

```c#
public class UserListData
{
   //...
   [JsonIgnore]
   public int? CompanyId { get; set; }
}
```
> You can use [JsonIgnore] attribute to prevent the property from being sent to the client.
   
```c#
var qb = new DataTableQueryBuilder<UserListData>(request, o =>
{
    o.ForField(f => f.CompanyName, o => {
        o.SearchBy((u, val) => u.CompanyId.HasValue && u.CompanyId == int.Parse(val));
    });
});
```

With this configuration the resulting query will look like this:

```c#
//IQueryable<UserListData> users = userService.GetAllForUserList();

return dataContext.Users
    .Select(u => new UserListData()
    {
        //...
        CompanyId = u.CompanyId
    })
    .Where(p => p.FullName.ToLower().Contains("John".ToLower()))
    .Where(p => p.CompanyId.HasValue && p.CompanyId == int.Parse("1"))
    .Where(p => p.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
    .OrderBy(p => p.Posts);
```

Similarly, you can use the `SortBy` method to set a custom sort expression.

## Usage without data projection

While using data projection in base LINQ query is fine for most use cases, you may find yourself in a situation when you do not want to or cannot use the data projection in the base query.

In such cases, you may return your EF entity instead of projection model from your base LINQ query, but introduce a separate view model instead.

1. Create a base LINQ query that will be used by query builder to request users from a database:

    ```c#
    public class UserService
    {
        public IQueryable<User> GetAllWithCompaniesAndPosts()
        {
            return dataContext.Users
                .Include(u => u.Company)
                .Include(u => u.Posts);
        }   
    }
    ```

2. Create a view model that represents the fields expected by your JS datatable and returned by server:

    ```c#
    public class UserDataTableFields
    {
        public int Id { get; set; }        
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Posts { get; set; }
    }
    ```

3. Create a mapping between `User` and `UserDataTableFields` that will be used to convert the data returned by LINQ query (`IEnumerable<User>`) to a JSON data array expected by datatable (`IEnumerable<UserDataTableFields>`):

    ```c#
    CreateMap<User, UserDataTableFields>()
        .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
        .ForMember(d => d.Posts, o => o.MapFrom(s => s.Posts.Count()));
    ```

4. Create an action that will receive a request from datatable, convert it to a LINQ query and return the data:

    ```c#
    public IActionResult UserList(DataTableRequest request)
    {
        var users = userService.GetAllWithCompaniesAndPosts();

        var qb = new DataTableQueryBuilder<UserDataTableFields, User>(request, o =>
        {
            o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
            o.ForField(f => f.Posts, o => {
                o.SearchBy((u, val) => u.Posts.Any(p => p.Title.ToLower().Contains(val.ToLower())));
                o.OrderBy(u => u.Posts.Count());
            });
        });

        var result = qb.Build(users);

        return result.CreateResponse(mapper);
    }
    ```

    > The CreateResponse method will use AutoMapper to convert the data returned by LINQ query to a JSON data array expected by datatable.

Let's review how this approach works with the following datatable request:

```js
search: [{'fullName' : 'John'}, {'companyName': 'Goo'}, {'posts': 'Some title'}]
sort: [{'posts' : 'asc'}]
```

Here, the `companyName` field's name doesn't match with any of the entity's property names, so we need to tell the builder which entity's property to use by utilizing the ``UseSourceProperty`` method:

```c#
o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
```

As described before, if we are unable to filter or sort the data just by matching the property's value, we can specify a LINQ expressions that will be used to filter/sort the data by using the ``SearchBy`` and ``OrderBy`` methods:

```c#
o.ForField(f => f.Posts, o => {
    o.SearchBy((u, val) => u.Posts.Any(p => p.Title.ToLower().Contains(val.ToLower())));
    o.OrderBy(u => u.Posts.Count());
});
```

With this configuration the resulting LINQ query will look like this:

  ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();

  return users
      .Where(u => u.FullName.ToLower().Contains("John".ToLower()))
      .Where(u => u.Company!.Name.ToLower().Contains("Goo".ToLower()))
      .Where(u => u.Posts.Any(p => p.Title.ToLower().Contains("Some title".ToLower()))
      .OrderBy(u => u.Posts.Count());
  ```
