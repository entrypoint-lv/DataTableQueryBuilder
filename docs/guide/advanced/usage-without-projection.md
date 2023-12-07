# Usage without Projection

While using projection in base LINQ query is fine for most use cases, you may find yourself in a situation where you don't want to or can't do that.

In such cases, you can return the *EF entity* from your base LINQ query, but introduce a separate view model that will represent the fields expected by datatable.

## Step 1. Create a view model

Create a strongly typed view model that represents the fields expected by your JS datatable and returned by server:

```c#
public class UserDataTableFields
{
    public int Id { get; set; }        
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int Posts { get; set; }
    public DateTime CreateDate { get; set; }
}
```

## Step 2. Create a base query

Create a base LINQ query that will be used by query builder to request users from a database:

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

## Step 3. Create a mapping

Use `AutoMapper` to create a mapping between `User` and `UserDataTableFields` that will be used by builder to map entity returned by query to a view model expected by datatable:

```c#
CreateMap<User, UserDataTableFields>()
    .ForMember(d => d.CompanyName, o => 
        o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty)
    )
    .ForMember(d => d.Posts, o => 
        o.MapFrom(s => s.Posts.Count())
    );
```

## Step 4. Create an action

Create an action that will receive an AJAX request from your JS datatable, transform it to a LINQ query and return the data.

Since we now use two different models, one returned by query, and one expected by datatable, we may need to provide an additional configuration:

```c#
public IActionResult UserList(DataTableRequest request)
{
    var users = userService.GetAllWithCompaniesAndPosts();

    var qb = new DataTableQueryBuilder<UserDataTableFields, User>(request, o =>
    {
        o.ForField(f => f.CompanyName, o => 
            o.UseSourceProperty(u => u.Company!.Name)
        );
        o.ForField(f => f.Posts, o => 
            o.SearchBy((u, val) => u.Posts.Count() == int.Parse(val));
            o.OrderBy(u => u.Posts.Count());
        );
    });

    var result = qb.Build(users);

    return result.CreateResponse(mapper);
}
```

## How it works

Let's review the same datatable request that we already reviewed before:

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

Here, `fullName` and `createDate` fields matches the properties of the source `User` entity, so we don't need to provide an additional configuration for them.

The `companyName` field doesn't match any property of the source `User` entity, but we can use `UseSourceProperty` method to explicitly set the property to be used when filtering and sorting.

Also, in order to be able to filter/sort by `posts` field, we need to explicitly set custom expressions:

```c#
o.ForField(f => f.CompanyName, o => 
    o.UseSourceProperty(u => u.Company!.Name)
);
o.ForField(f => f.Posts, o =>
    o.SearchBy((u, val) => u.Posts.Count() == int.Parse(val));
    o.OrderBy(u => u.Posts.Count());
);
```

With this configuration the resulting LINQ query will look like this:

  ```c#
  //IQueryable<User> users = userService.GetAllWithCompaniesAndPosts();

  return dataContext.Users
      .Include(u => u.Company)
      .Include(u => u.Posts)
      .Where(u => u.FullName.ToLower().Contains("John".ToLower()))
      .Where(u => u.Company!.Name.ToLower().Contains("Goo".ToLower()))
      .Where(p => p.Posts.Count() == int.Parse("5"))
      .Where(u => u.CreateDate.Date == DateTime.ParseExact("05/15/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture))
      .OrderBy(u => u.Posts.Count())
      .Skip(0).Take(20);
  ```

## Pros and cons of usage without projection

The main adavantage of using builder without projection is the advanced filtering capabilities since we are not limited to just the properties of projection model.

Since we have the full access to the source entity and its navigation properties we may easily implement an advanced filtering that is not possible when using projection model.

For example, we may want to filter `posts` by their titles rather than by count:
  
```js
columns: [
    { field: 'posts', search: 'some title', sort: 'asc' },
],
page: 1,
pazeSize: 20
```

This is easily achievable using a custom search expression:

```c#
o.ForField(f => f.Posts, o =>
    o.SearchBy((u, val) => u.Posts.Any(p => p.Title.ToLower().Contains(val.ToLower())));
    o.OrderBy(u => u.Posts.Count());
);
```

Another benefit is that we can perform data transformations when mapping the source entity to the view model.

For example, if we have a `CreateDate` property of type `DateTime` on the source entity, we can easily convert it to a formatted string to display it in a datatable:

```c#
CreateMap<User, UserDataTableFields>()
    .ForMember(d => d.CreateDate, o => 
        o.MapFrom(s => s.CreateDate.ToString("MMMM dd, yyyy"))
    );
```

The main disadvantage is an increased complexity due to the need for mapping and additional builder configuration.