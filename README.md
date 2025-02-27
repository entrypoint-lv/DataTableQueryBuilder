# Server-side .NET query builder for JavaScript data tables

With this builder, you can implement server-side paging, filtering and sorting for any JavaScript datatable with just a few lines of code.

The builder automatically transforms an AJAX request coming from a JavaScript datatable into a LINQ query against the Entity Framework data model according to the provided configuration.

It can be used with any JavaScript datatable component that supports server-side processing (currently tested on [datatables.net](https://datatables.net) and [vue-good-table](https://xaksis.github.io/vue-good-table/) only).

## Docs

The full documentation is available here - https://entrypoint-lv.github.io/DataTableQueryBuilder/

## Demo and samples

A [live demo](https://codesandbox.io/s/datatablesnet-with-datatablequerybuilder-hgpg2) of using [DataTables](https://datatables.net) with DataTableQueryBuilder.

A [live demo](https://codesandbox.io/s/vue-good-table-with-datatablequerybuilder-cynse) of using [vue-good-table](https://xaksis.github.io/vue-good-table/) with DataTableQueryBuilder.

A [source code](https://github.com/entrypoint-lv/DataTableQueryBuilder/tree/master/samples/SampleAPI) and [OpenAPI specification](https://query-builder-sample-api.entrypointdev.com/swagger/) of server-side API that is used in the above demos.

## Install

If you're using [DataTables](https://datatables.net) or wrappers around it, install the [DataTableQueryBuilder.DataTables](https://www.nuget.org/packages/datatablequerybuilder.datatables/) NuGet package:

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

For other JavaScript datatable components, install the [DataTableQueryBuilder.Generic](https://www.nuget.org/packages/datatablequerybuilder.generic/) NuGet package instead:

```console
dotnet add package DataTableQueryBuilder.Generic
```

Nothing else is needed.

## Basic usage

Let's suppose that you want to show a searchable and sortable list of users, with all sorting, paging and filtering happening on the server-side.

In case of using Datatables, your configuration will look something like this:

```html
<div id="filters">
    <input type="text" data-column="id" placeholder="Id" />
    <input type="text" data-column="fullName" placeholder="Full Name" />
    <input type="text" data-column="email" placeholder="Email" />
    <input type="text" data-column="posts" placeholder="Posts" />
    <input type="text" data-column="createDate" placeholder="MM/DD/YYYY" />
</div>

<table id="user-list">
    <thead>
    <tr>
        <th>Id</th>
        <th>Full name</th>
        <th>Email</th>
        <th>Company</th>
        <th>Posts</th>
        <th>Create Date</th>
    </tr>
    </thead>
</table>

<script>
const apiUrl = "https://query-builder-sample-api.entrypointdev.com/API/UserList.DataTables";

$(document).ready(function () {
    let dt = $("#user-list").DataTable({
       processing: true,
       serverSide: true,
       ajax: {
           url: apiUrl,
           type: "POST"
       },
       columns: [
           { name: "id", data: "id" },
           { name: "fullName", data: "fullName" },
           { name: "email", data: "email" },
           { name: "companyName", data: "companyName" },
           { name: "posts", data: "posts" },
           { name: "createDate", data: "createDate" }
       ]
    });

    $("#filters input").each(function () {
        let columnName = $(this).data("column");

        $(this).on("change", function () {
            let col = dt.column(columnName + ":name");

            if (col.search() !== this.value)
                col.search(this.value).draw();
        });
    });
</script>
```

Your datatable will send requests to the back-end and expect server to return the correct rows (in form of a JSON array) to display them in the UI.

A `data` property in column configuration contains a property name of a row object in the returned JSON array, for example:

```js
[
    {
        'id': 1,
        'fullName': 'John Smith',
        'email': 'john@example.com',
        'companyName': '',
        'posts' : 0,
        'createDate': '2021-01-05T19:38:23.551Z'
    }
    {
        'id': 2,
        'fullName': 'Michael Smith',
        'email': 'michael@example.com',
        'companyName': 'Apple',
        'posts' : 5,
        'createDate': '2021-04-23T18:15:43.511Z'
    }
    {
        'id': 3,
        'fullName': 'Mary Smith',
        'email': 'mary@example.com',
        'companyName': 'Google',
        'posts' : 10,
        'createDate': '2020-09-12T10:11:45.712Z'
    }
]
```

### Step 1. Create Entity Framework data model

Create your Entity Framework data model. We'll use the following simple data model in this example:
  
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

### Step 2. Create projection model

Create a strongly typed projection model that represents the fields expected by your JS datatable and returned by server:

```c#
public class UserListData
{
    public int Id { get; set; }        
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public int Posts { get; set; }
    public DateTime CreateDate { get; set; }
}
```

### Step 3. Create a base query

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
            CompanyName = u.Company != null ? u.Company.Name : "",
            Posts = u.Posts.Count(),
            CreateDate = u.CreateDate
        });
    }   
}
```

### Step 4. Create an action

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

That's all!

Please refer to [the documentation](https://entrypoint-lv.github.io/DataTableQueryBuilder/) for available configuration options and detailed information.
