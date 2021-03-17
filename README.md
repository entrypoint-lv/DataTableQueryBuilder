# LINQ query builder for front-end datatables

 A LINQ query builder that automatically converts AJAX request coming from front-end datatable to LINQ query against the Entity Framework model according to the specified configuration.

# Basic usage

Let's assume that we have some front-end datatable that represents a list of users:

```html
<my-datatable-component :data-url="/api/userlist" :fields="fields" />

<script>
    let fields = [
        'Id',
        'FullName',
        'Email',
        'CompanyName',
        'Role',
    ];
</script>
```

And our Entity Framework data model looks like this:

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

1. Create a LINQ query that will be used by query builder to request users from a database:

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

2. Create a view model that represents fields of a front-end datatable. This model will also be used to return data from the server to the datatable:

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

3. Create an action that will receive an AJAX request from front-end datatable, convert it to a LINQ query and return the data:

    ```c#
    public IActionResult UserList(DataTablesRequest request)
    {
        var qb = new DataTablesQueryBuilder<UserDataTableFields, User>(request, o =>
        {
            o.ForField(f => f.CompanyName, o =>
            {
                o.UseSourceProperty(u => u.Company!.Name);
            });
            o.ForField(f => f.Role, o => o.SearchBy((u, val) => u.Roles.Any(r => r.RoleId == int.Parse(val))));
        });

        var users = userService.GetAllWithRolesAndCompanies();

        var result = qb.Build(users);

        return result.MapToResponse(mapper);
    }
    ```

# Configuring

The front-end datatable knows nothing about the server-side data source - it only knows the list of its fields (columns), represented by ``UserDataTableFields`` model.

When user applies search to some column, the datatable sends a request to the server that includes a filtering clause, for example, ``FullName`` = 'some value'.

The task of the query builder is to extend an existing LINQ query with an additional ``Where`` clause to filter the data.

But since ``FullName`` is just the name of a field in the front-end datatable, it needs to know which property of the ``User`` entity corresponds to that field.

- By default, the builder assumes that the name of a field matches the name of a property of the ``User`` entity and will extend an existing LINQ query so that resulting query will look like this:

  ```c#
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
      .Where(u => u.FullName.Contains(val));
  ```

  No configuration is required in this case.

- Sometimes the field and property names do not match, like with the ``CompanyName`` field.

  In this case, we need to tell the builder which entity's property to use by utilizing the ``UseSourceProperty`` method:

  ```c#
  o.ForField(f => f.CompanyName, o =>
  {
      o.UseSourceProperty(u => u.Company!.Name);
  });
  ```

  With this configuration the resulting LINQ query will look like this:

  ```c#
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
      .Where(u => u.Company!.Name.Contains(val));
  ```

- Sometimes there is no corresponding property in the entity at all.

  In this case, we can specify a LINQ expression that will be used to filter (or sort) on this field by using the ``SearchBy`` and ``OrderBy`` methods:

  ```c#
  o.ForField(f => f.Role, o => o.SearchBy((u, val) => u.Roles.Any(r => r.RoleId == int.Parse(val))));
  ```

  This expression will be added to the LINQ query so that resulting query will look like this:
  
    ```c#
  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Roles)
          .ThenInclude(r => r.Role)
      .Where(u => u.Roles.Any(r => r.RoleId == int.Parse(val));
  ```

# Executing

After configuration is provided, the query can be builded by calling the ``Build`` method. 

```c#
var result = qb.Build(users);
```

This method returns a ``BuildResult`` object that contains a builded query and some other properties, expected by front-end datatable. Please note, that builded query is not executed yet.

To execute the query and return the data to the datatable, call the ``MapToResponse`` method:

```c#
return result.MapToResponse(mapper);
```

This method will use AutoMapper to convert the data returned by LINQ query (``IEnumerable<User>``) to the format expected by datatable (``IEnumerable<UserDataTableFields>``), so don't forget to create a mapping between ``User`` and ``UserDataTableFields``:

```c#
CreateMap<User, UserDataTableFields>()
    .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
    .ForMember(d => d.Role, o => o.MapFrom(s => string.Join(", ", s.Roles.Select(r => r.Role.Name))));
```
