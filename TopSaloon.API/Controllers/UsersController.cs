﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseResultHandlerController<UsersService>
    {
        public UsersController(UsersService _service) : base(_service)
        {

        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            return await AddItemResponseHandler(async () => await service.CreateRole(roleName));
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            return await AddItemResponseHandler(async () => await service.AssignRole(userId, roleName));
        }

        [HttpPost("CreateAdminAccount")]
        public async Task<IActionResult> CreateAdmin(AdminCreationModel model)
        {
            return await AddItemResponseHandler(async () => await service.CreateAdmin(model));
        }

        [HttpPost("DeleteAdmin")]
        public async Task<IActionResult> DeleteAdmin(string adminId)
        {
            return await AddItemResponseHandler(async () => await service.DeleteAdmin(adminId));
        }

        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLoginAsync(AdminLoginModel model)
        {
            return await GetResponseHandler(async () => await service.LoginAsync(model));
        }
        //GetUserDailyEarningPerTime
        [HttpPost("GetUserDailyEarningPerTime")]
        public async Task<IActionResult> GetUserDailyEarningPerTime(DateTime Start, DateTime End)
        {
            return await GetResponseHandler(async () => await service.GetUserDailyEarningPerTime(Start , End));
        }
    }
}
