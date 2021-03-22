# LINQ query builder for JavaScript data tables

 A LINQ query builder that automatically converts AJAX request coming from a front-end datatable to LINQ query against the Entity Framework data model according to the specified configuration.

# Basic usage

Let's assume that we have some front-end datatable that represents a list of users:

```html
<my-datatable-component :data-url="/api/userlist" :columns="columns"></my-datatable-component>

<script>
    let columns = [
        { title: 'Id', field: 'id' },
        { title: 'Full Name', field: 'fullName' },
        { title: 'Email', field: 'email' },
        { title: 'Company Name', field: 'companyName' },
        { title: 'Posts', field: 'posts' }
    ];
</script>
```

A field value is the property name of a row object in a JSON data array returned by server:

```js
    [
        { 'id': 1, 'fullName': 'John Smith', 'email': 'john@example.com', 'companyName': '', 'posts' : 0 }
        { 'id': 2, 'fullName': 'Michael Smith', 'email': 'michael@example.com', 'companyName': 'Apple', 'posts' : 5 }
        { 'id': 3, 'fullName': 'Mary Smith', 'email': 'mary@example.com', 'companyName': 'Google', 'posts' : 10 }
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

You need to:
   
1. Create a LINQ query that will be used by query builder to request users from a database:

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

4. Register model binder to bind the incoming AJAX request from datatable to a DataTablesRequest model:

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

5. Create an action that will receive a request from datatable, convert it to a LINQ query and return the data:

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

        return result.MapToResponse(mapper);
    }
    ```

# Configuring

The front-end datatable knows nothing about the Entity Framework data model - it only knows what fields to expect in the data returned by the server.

So, when user tries to filter or sort on some column, the datatable sends a request to the server that includes a filtering and ordering clauses, for example:
```js
searchByField: 'fullName'
searchValue: 'some value'
sortByField: 'id'
sortDirection: 'asc'
```

The task of the query builder is to extend an existing LINQ query with an additional ``Where`` and ``OrderBy`` clauses based on the request.

- By default, if no any configuration is provided, the builder will extend an existing query in the following way:

  ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();
  
  return users
      .Where(u => u.FullName.Contains(val))
      .OrderBy(u => u.Id);
  ```

  Here, the property names ``FullName`` and ``Id`` were figured out automatically based on the field names in the request (ignoring the case sensivity).

- If field and property names do not match, like with the ``CompanyName`` field, we need to tell the builder which entity's property to use by utilizing the ``UseSourceProperty`` method:

  ```c#
  o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
  ```

  With this configuration the resulting LINQ query will look like this:

  ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();

  return users
      .Where(u => u.Company!.Name.Contains(val));
      .OrderBy(u => u.Id);
  ```

- Sometimes we are unable to filter or sort the data just by matching the property's value.

  In this case, we can specify a LINQ expressions that will be used to filter/sort the data by using the ``SearchBy`` and ``OrderBy`` methods:

  ```c#
  o.ForField(f => f.Posts, o => {
      o.SearchBy((u, val) => u.Posts.Any(p => p.Title.Contains(val)));
      o.OrderBy(u => u.Posts.Count());
  });
  ```

  These expressions will be added to the LINQ query so that the resulting query will look like this:
  
    ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();

  return users
      .Where(u => u.Posts.Any(p => p.Title.Contains(val))
      .OrderBy(u => u.Posts.Count());
  ```

# Executing

The query can be builded by calling the ``Build`` method. 

```c#
var result = qb.Build(users);
```

This method returns a ``BuildResult`` object that contains a builded query and some other properties, expected by front-end datatable. Please note, that builded query is not executed yet.

To execute the query and return the data to the datatable, call the ``MapToResponse`` method:

```c#
return result.MapToResponse(mapper);
```

This method will use AutoMapper to convert the data returned by LINQ query (``IEnumerable<User>``) to a JSON data array expected by datatable (``IEnumerable<UserDataTableFields>``).
