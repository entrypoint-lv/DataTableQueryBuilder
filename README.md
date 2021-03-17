# LINQ query builder for front-end datatables

 A LINQ query builder that automatically converts AJAX request coming from front-end datatable to LINQ query against the Entity Framework data model according to the specified configuration.

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
        { title: 'Role', field: 'role' }
    ];
</script>
```

A field value is the property name of a row object in a JSON data array returned by server:

```js
    [
        { 'id': 1, 'fullName': 'John Smith', 'email': 'john@example.com', 'companyName': '', 'role' : 'Coordinator' }
        { 'id': 2, 'fullName': 'Michael Smith', 'email': 'michael@example.com', 'companyName': 'Apple', 'role' : 'Coordinator, Manager' }
        { 'id': 3, 'fullName': 'Mary Smith', 'email': 'mary@example.com', 'companyName': 'Google', 'role' : 'Manager, Admin' }
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

    public virtual ICollection<UserRole> Roles { get; } = new List<UserRole>();
}

public class Company
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
```

In this case we need to create the following:

1. A LINQ query that will be used by query builder to request users from a database:

    ```c#
    public class UserService
    {
        public IQueryable<User> GetAllWithRolesAndCompanies()
        {
            return dataContext.Users
                .Include(u => u.Company)
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Role);
        }   
    }
    ```

2. A view model that represents fields returned by server:

    ```c#
    public class UserDataTableFields
    {
        public int Id { get; set; }        
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
    ```

3. An action that will receive an AJAX request from front-end datatable, convert it to a LINQ query and return the data:

    ```c#
    public IActionResult UserList(DataTablesRequest request)
    {
        var qb = new DataTablesQueryBuilder<UserDataTableFields, User>(request, o =>
        {
            o.ForField(f => f.CompanyName, o => o.UseSourceProperty(u => u.Company!.Name));
            o.ForField(f => f.Role, o => o.SearchBy((u, val) => u.Roles.Any(r => r.RoleId == int.Parse(val))));
        });

        var users = userService.GetAllWithRolesAndCompanies();

        var result = qb.Build(users);

        return result.MapToResponse(mapper);
    }
    ```

# Configuring

The front-end datatable knows nothing about the Entity Framework data model - it only knows what fields to expect in the data returned by the server.

So, when user tries to filter or sort some column, the datatable sends a request to the server that includes a filtering and ordering clauses, for example:
```js
searchByField: 'fullName'
searchValue: 'some value'
sortByField: 'id'
sortDirection: 'asc'
```

The task of the query builder is to extend an existing LINQ query with an additional ``Where`` and ``OrderBy`` clauses based on the request.

- By default, if no any configuration is provided, the builder will extend an existing query in the following way:

  ```c#
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
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
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
      .Where(u => u.Company!.Name.Contains(val));
      .OrderBy(u => u.Id);
  ```

- Sometimes we are unable to filter or sort the data just by matching the property's value.

  In this case, we can specify a LINQ expressions that will be used to filter/sort the data by using the ``SearchBy`` and ``OrderBy`` methods:

  ```c#
  o.ForField(f => f.Role, o => {
      o.SearchBy((u, val) => u.Roles.Any(r => r.Role.Id == int.Parse(val)));
      o.OrderBy(u => u.Roles.Select(r => r.Role.Name));
  });
  ```

  These expressions will be added to the LINQ query so that the resulting query will look like this:
  
    ```c#
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
      .Where(u => u.Roles.Any(r => r.RoleId == int.Parse(val))
      .OrderBy(u => u.Roles.Select(r => r.Role.Name));
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

This method will use AutoMapper to convert the data returned by LINQ query (``IEnumerable<User>``) to a JSON data array expected by datatable (``IEnumerable<UserDataTableFields>``), so don't forget to create a mapping between ``User`` and ``UserDataTableFields``:

```c#
CreateMap<User, UserDataTableFields>()
    .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
    .ForMember(d => d.Role, o => o.MapFrom(s => string.Join(", ", s.Roles.Select(r => r.Role.Name))));
```
