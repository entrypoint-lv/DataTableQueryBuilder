using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataTableQueryBuilder.DataTables;

namespace SampleAPI.Controllers
{
    using Models.Data;
    using Services;

    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly IUserService userService;

        private readonly ILogger<APIController> logger;

        public APIController(IUserService userService, ILogger<APIController> logger)
        {
            this.userService = userService;
            this.logger = logger;
        }

        [HttpPost]
        [Route("UserList")]
        public IActionResult UserList([FromForm] DataTablesRequest request)
        {
            var users = userService.GetAllForUserList();

            var qb = new DataTablesQueryBuilder<UserListData>(request);

            var result = qb.Build(users);

            return result.CreateResponse();
        }

        [HttpPost]
        [Route("UserList2")]
        public IActionResult UserList2([FromForm] DataTablesRequest request)
        {
            var users = userService.GetAllForUserList();

            var qb = new DataTablesQueryBuilder<UserListData>(request);

            var result = qb.Build(users);

            return result.CreateResponse();
        }

    }
}
