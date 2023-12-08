# Basic Usage

Let's suppose that you want to show a searchable and sortable list of users, with all sorting, paging and filtering happening on the server-side.

In case of using [DataTables](https://datatables.net), your configuration will look something like this:

::: code-group

```js [JavaScript]
const apiUrl =
    "https://query-builder-sample-api.entrypointdev.com/API/UserList.DataTables";

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
});
```

```html [HTML]
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
```

:::

Your datatable will send requests to the back-end and expect the server to return the correct rows (in form of a JSON array) to display them in the UI.

The `data` property in the column configuration points to a property of an element in the returned JSON array, for example:

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

::: tip Note
From now on, we will use the term `field` to refer to the property in the returned JSON array.
:::

## Step 1. Create an Entity Framework data model

Create your Entity Framework data model. We'll use the following simple data model in all examples:
  
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

## Step 2. Create a projection model

Create a strongly typed projection model that represents the fields expected by your JS datatable and returned by the server:

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

::: tip Note
The `CreateDate` property is of type `DateTime`, so it will be returned in an ISO date format.

This is intentional, as in our case the formatting will happen in the UI and not in the LINQ query.
:::

## Step 3. Create a base query

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

## Step 4. Create an action

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

::: tip Note
The Build method doesn't execute the query - it returns a `BuildResult` object that contains a builded query and some other properties, expected by datatable.

To execute the query and return the data to the datatable, call the `CreateResponse` method.
:::

## Sandbox demo

Check out the [sandbox demo](https://codesandbox.io/s/datatablesnet-with-datatablequerybuilder-hgpg2) of the example above:

<iframe src="https://codesandbox.io/embed/datatablesnet-with-datatablequerybuilder-hgpg2?fontsize=14&hidenavigation=1&theme=dark"
     style="width:100%; height:800px; border:0; border-radius: 4px; overflow:hidden;"
     title="DataTables with DataTableQueryBuilder"
     allow="accelerometer; ambient-light-sensor; camera; encrypted-media; geolocation; gyroscope; hid; microphone; midi; payment; usb; vr; xr-spatial-tracking"
     sandbox="allow-forms allow-modals allow-popups allow-presentation allow-same-origin allow-scripts"
   ></iframe>
