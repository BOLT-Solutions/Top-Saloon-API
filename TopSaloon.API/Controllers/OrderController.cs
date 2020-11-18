using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.ServiceLayer;
using TopSaloon.Repository.Common;

namespace TopSaloon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseResultHandlerController<OrdersService>
    {
        public OrderController(OrdersService _service) : base(_service)
        {

        }

        [HttpGet("GetOrderServices/{id}")]
        public async Task<IActionResult> GetOrderServicesViaOrderId(int id)
        {
            return await GetResponseHandler(async () => await service.GetOrderServicesViaOrderId(id));
        }

        [HttpGet("GetCompleteOrderById/{id}")]
        public async Task<IActionResult> GetCompleteOrderById(int id)
        {
            return await GetResponseHandler(async () => await service.GetCompleteOrderById(id));
        }


        [HttpPut("SetOrderService")]
        public async Task<IActionResult> SetOrderService(int orderServiceId)
        {
            return await EditItemResponseHandler(async () => await service.SetOrderService(orderServiceId));
        }
        //[HttpPost("GoogleSheets")]
        //public async Task<IActionResult> GoogleSheets(OrderToRecord orderToRecord)
        //{
        //    return await AddItemResponseHandler(async () => service.AddOrderToGoogleSheets(orderToRecord));
        //}

        [HttpPut("CancelOrder")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            return await EditItemResponseHandler(async () => await service.CancelOrder(orderId));
        }
        [HttpGet("FinalizeOrder/{id}")]
        public async Task <IActionResult> FinalizeOrder(int id)
        {
            return await GetResponseHandler(async () => await service.FinalizeOrder(id));
        }
        [HttpPut("ConfirmOrderServices")]
        public async Task<IActionResult> ConfirmOrderServices(List<OrderServiceDTO> orderServices)
        {
            return await EditItemResponseHandler(async () => await service.ConfirmOrderServices(orderServices));
        }
        //getAllCompleteOrderByDate
        [HttpGet("GetAllCompleteOrderByDate/{date}")]
        public async Task<IActionResult> getAllCompleteOrderByDate(DateTime date)
        {
            return await GetResponseHandler(async () => await service.getAllCompleteOrderByDate(date));
        }
        [HttpGet("StartOrder/{id}")]
        public async Task<IActionResult> StartOrder(int id)
        {
            return await GetResponseHandler(async () => await service.StartOrder(id));
        }

    }
}
