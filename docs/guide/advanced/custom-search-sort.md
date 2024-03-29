﻿# Custom Search and Sort Expressions

Sometimes you may want to filter data in a column based on a value that doesn't belong to that column.

A common example would be the `CompanyName` column, that needs to be filtered by `companyId` value from `<select>` element that exists on the same page.

One way to achieve this is to add the `CompanyId` property to the projection model:

::: code-group

```c#{6} [Projection Model]
public class UserListData
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public int? CompanyId { get; set; }
    public string CompanyName { get; set; } = "";
    public int Posts { get; set; }
    public DateTime CreateDate { get; set; }
}
```

```c#{11} [Base Query]
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
            CompanyId = u.CompanyId,
            CompanyName = u.Company != null ? u.Company.Name : "",
            Posts = u.Posts.Count(),
            CreateDate = u.CreateDate
        });
    }   
}
```

:::

::: info 
You can use [JsonIgnore] attribute to prevent the property from being sent to the client.
:::
   
 And then use the `SearchBy` configuration method to specify a custom LINQ expression that should be used when filtering by `companyName` field:

```c#
var qb = new DataTableQueryBuilder<UserListData>(request, o =>
{
    o.ForField(f => f.CompanyName, o => {
        o.SearchBy((u, val) => u.CompanyId.HasValue && u.CompanyId == int.Parse(val));
    });
});
```

With this configuration the base LINQ query will be extended in the following way:

```c#
return dataContext.Users
    //your base query
    .Select(u => new UserListData()
    {
        //...
        CompanyId = u.CompanyId
        //...
    })
    //generated part
    //...
    .Where(p => p.CompanyId.HasValue && p.CompanyId == int.Parse("1"))
    //...
```

Similarly, you can use the `OrderBy` method to set a custom sort expression.