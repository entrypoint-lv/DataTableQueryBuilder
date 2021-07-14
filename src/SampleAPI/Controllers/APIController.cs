﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [Route("UserList.DataTables")]
        public IActionResult UserList([FromForm] DataTableQueryBuilder.DataTables.DataTablesRequest request)
        {
            var users = userService.GetAllForUserList();

            var qb = new DataTableQueryBuilder.DataTables.DataTableQueryBuilder<UserListData>(request);

            var result = qb.Build(users);

            return result.CreateResponse();
        }

        [HttpPost]
        [Route("UserList.Generic")]
        public IActionResult UserList2([FromForm] DataTableQueryBuilder.Generic.DataTableRequest request)
        {
            var users = userService.GetAllForUserList();

            var qb = new DataTableQueryBuilder.Generic.DataTableQueryBuilder<UserListData>(request);

            var result = qb.Build(users);

            return result.CreateResponse();
        }

    }
}