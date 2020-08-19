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
    public class DailyReportController : BaseResultHandlerController<DailyReportService>
    {
        public DailyReportController(DailyReportService _service) : base(_service)
        {

        }

        [HttpPost("SaveDailyReport")]

        public async Task<IActionResult> SaveDailyReport(DailyReportDTO dailyReport)
        {
            return await AddItemResponseHandler(async () => await service.SaveDailyReport(dailyReport));
        }

        [HttpGet("GetTotalNumberCustomerForToday")]

        public async Task<IActionResult> GetTotalNumberCustomerForToday( )
        {
            return await GetResponseHandler(async () => await service.GetTotalNumberCustomerForToday( ));
        }
        [HttpGet("GetTotalServiceCostForToday")]

        public async Task<IActionResult> GetTotalServiceCostForToday( )
        {
            return await GetResponseHandler(async () => await service.GetTotalServiceCostForToday( ));
        }
        [HttpGet("GetAverageOfWaitingTimeForToday")]

        public async Task<IActionResult> GetAverageOfWaitingTimeForToday( )
        {
            return await GetResponseHandler(async () => await service.GetAverageOfWaitingTimeForToday(  ));
        }
        [HttpGet("GetNumberOfSignedInBarbersForToday")]

        public async Task<IActionResult> GetNumberOfSignedInBarbersForToday()
        {
            return await GetResponseHandler(async () => await service.GetNumberOfSignedInBarbersForToday( ));
        }
    }
}
