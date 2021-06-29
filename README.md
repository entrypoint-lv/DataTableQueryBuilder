# LINQ query builder for JavaScript data tables

 A LINQ query builder that automatically converts AJAX request coming from a front-end datatable to a LINQ query against the Entity Framework data model according to the specified configuration.

## Install

### DataTables.net

If you're using https://datatables.net, install the following NuGet package:

   ```console
   dotnet add package DataTableQueryBuilder.DataTables
   ```

Then register the model binder to bind incoming AJAX requests from DataTables to a DataTablesRequest model:

   ```c#
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

### Other datatable components

For other datatable components, simply install the basic package instead:

```console
dotnet add package DataTableQueryBuilder
```

## Example setup

Let's assume that we have some front-end datatable that represents a list of users:

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

Here, a field value is the property name of a row object in a JSON data array returned by the server, for example:

```js
    [
        { 'id': 1, 'fullName': 'John Smith', 'email': 'john@example.com', 'companyName': '', 'posts' : 0, createDate: '2021-01-05T19:38:23.551Z' }
        { 'id': 2, 'fullName': 'Michael Smith', 'email': 'michael@example.com', 'companyName': 'Apple', 'posts' : 5, createDate: '2021-04-23T18:15:43.511Z' }
        { 'id': 3, 'fullName': 'Mary Smith', 'email': 'mary@example.com', 'companyName': 'Google', 'posts' : 10, createDate: '2020-09-12T10:11:45.712Z' }
    ]
```

Let's assume that our Entity Framework data model looks like this:

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

## Basic usage

In simple cases you can use LINQ projection to return fields required by datatable directly from a LINQ query.

With this approach you need to:

1. Create a projection model that represents the fields returned by server:

    ```c#
    public class UserListData
    {
        public int Id { get; set; }        
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int? CompanyId { get; set; };
        public int Posts { get; set; }
        public DateTime CreateDate { get; set; }
    }
    ```
    
    > Note that the ``CreateDate`` field will be returned in ISO date format and not as formatted string - this is intentional, as the formatting must happen in the UI and not in the LINQ query.

2. Create a base LINQ query that will be used by query builder to request users from a database. Use the projection to select the required data:

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
                CompanyId = u.CompanyId,
                Posts = u.Post.Count(),
                CreateDate = u.CreateDate
            });
        }   
    }
    ```

5. Create an action that will receive a request from datatable, convert it to a LINQ query and return the data:

    ```c#
    public IActionResult UserList(DataTablesRequest request)
    {
        var qb = new DataTablesQueryBuilder<UserListData>(request);

        var users = userService.GetAllForUserList();

        var result = qb.Build(users);

        return result.CreateResponse();
    }
    ```
    
    > The Build method returns a BuildResult object that contains a builded query and some other properties, expected by front-end datatable. Please note that this method doesn't execute the query. To execute the query and return the data to the datatable, call the CreateResponse method.

## How it works

When user applies filtering or sorting to some columns, the datatable sends a request to the server that includes a filtering and ordering clauses.

For example, the request could look something like this:
```js
search: [{'fullName' : 'John'}, {'companyName': 'Goo'}]
sort: [{'posts' : 'asc'}]
```

The task of the query builder is to extend a base LINQ query with an additional ``Where`` and ``OrderBy`` clauses based on this request.

If no configuration is provided, the builder will automatically determine what match methods to use based on the properties' data types, so the base query will be extended in the following way:

```c#
//IQueryable<UserListData> users = userService.GetAllForUserList();

return dataContext.Users
    .Select(u => new UserListData()
    {
        ///
    })
    .Where(p => p.FullName.Contains(val))
    .Where(p => p.CompanyName.Contains(val))
    .OrderBy(p => p.Posts);
```

Sometimes you may want to filter data in a column based on some other field's data. A common example would be a ``<select>`` element that allows to filter by specific company and uses company's Id as selected value.

In this case, you can use the ``SearchBy`` method to specify a LINQ expressions that should be used when filtering by this field:

```c#
var qb = new DataTablesQueryBuilder<UserListData>(request, o =>
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
        ///
    })
    .Where(p => p.FullName.Contains(val))
    .Where(p => p.CompanyId.HasValue && p.CompanyId == int.Parse(val))
    .OrderBy(p => p.Posts);
```

## Advanced filtering

While returning fields directly from base query by using projection is fine for simple use cases, you may find that this approach doesn't allow you to perform a more advanced data filtering.

An example would be to show only those users that have a posts with a title that matches some value.

In these cases you should use the builder without projection and introduce a separate view model instead.

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

2. Create a view model that represents fields returned by server:

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

3. Create a mapping between ``User`` and ``UserDataTableFields`` that will be used to convert the data returned by LINQ query (``IEnumerable<User>``) to a JSON data array expected by datatable:

    ```c#
    CreateMap<User, UserDataTableFields>()
        .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
        .ForMember(d => d.Posts, o => o.MapFrom(s => s.Posts.Count()));
    ```

4. Create an action that will receive a request from datatable, convert it to a LINQ query and return the data:

    ```c#
    public IActionResult UserList(DataTablesRequest request)
    {
        var qb = new DataTablesQueryBuilder<UserDataTableFields, User>(request, o =>
        {
            o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
            o.ForField(f => f.Posts, o => {
                o.SearchBy((u, val) => u.Posts.Any(p => p.Title.Contains(val)));
                o.OrderBy(u => u.Posts.Count());
            });
        });

        var users = userService.GetAllWithCompaniesAndPosts();

        var result = qb.Build(users);

        return result.CreateResponse(mapper);
    }
    ```

    > The CreateResponse method will use AutoMapper to convert the data returned by LINQ query (``IEnumerable<User>``) to a JSON data array expected by datatable (``IEnumerable<UserDataTableFields>``).

Let's review an example when datatable sends the following request:

```js
search: [{'fullName' : 'John'}, {'companyName': 'Goo'}, {'posts': 'Title'}]
sort: [{'posts' : 'asc'}]
```

Here, the entity's property names ``FullName`` and ``Id`` will be figured out automatically based on the field names in the request (ignoring the case sensivity).

If field name and entity's property name don't match, like with our ``CompanyName`` field, we need to tell the builder which entity's property to use by utilizing the ``UseSourceProperty`` method:

```c#
o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
```

If we are unable to filter or sort the data just by matching the property's value, we can specify a LINQ expressions that will be used to filter/sort the data by using the ``SearchBy`` and ``OrderBy`` methods:

```c#
o.ForField(f => f.Posts, o => {
    o.SearchBy((u, val) => u.Posts.Any(p => p.Title.Contains(val)));
    o.OrderBy(u => u.Posts.Count());
});
```

With this configuration the resulting LINQ query will look like this:

  ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();

  return users
      .Where(u => u.FullName.Contains(val))
      .Where(u => u.Company!.Name.Contains(val))
      .Where(u => u.Posts.Any(p => p.Title.Contains(val))
      .OrderBy(u => u.Posts.Count());
  ```
