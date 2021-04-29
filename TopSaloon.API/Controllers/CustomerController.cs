using Microsoft.AspNetCore.Mvc;
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
    public class CustomerController : BaseResultHandlerController<CustomerService>
    {
        public CustomerController(CustomerService _service) : base(_service)
        {

        }

        [HttpPost("AddNewCustomer")]
        public async Task<IActionResult> AddNewCustomer(AddCustomerModel model)
        {
            return await AddItemResponseHandler(async () => await service.AddNewCustomer(model));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(CustomerLoginDTO loginRequest)
        {
            return await AddItemResponseHandler(async () => await service.Login(loginRequest));
        }
        [HttpGet("GuestLogin")]
        public async Task<IActionResult> GuestLogin()
        {
            return await GetResponseHandler(async () => await service.GuestLogin());
        }
        [HttpGet("GetCustomerTotalNumberOfVisit/{id}")]
        public async Task<IActionResult> GetCustomerTotalNumberOfVisit(int CustomerId)
        {
            return await AddItemResponseHandler(async () => await service.GetCustomerTotalNumberOfVisit(CustomerId));
        }
        [HttpGet("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            return await AddItemResponseHandler(async () => await service.GetCustomerById(id));
        }
        [HttpGet("GetCustomerVisitDetails/{id}")]
        public async Task<IActionResult> GetCustomerVisitDetails(int CustomerID)
        {
            return await GetResponseHandler(async () => await service.GetCustomerVisitDetails(CustomerID));
        }
        [HttpGet("GetNumberOfCustomerVisitForToday")]
        public async Task<IActionResult> GetCustomerVisitDetails(DateTime date)
        {
            return await GetResponseHandler(async () => await service.GetNumberOfCustomerVisitForToday(date));
        }
        [HttpGet("TotalAmountOfServiceCostForToday")]
        public async Task<IActionResult> TotalAmountOfServiceCostForToday(DateTime date)
        {
            return await GetResponseHandler(async () => await service.TotalAmountOfServiceCostForToday(date));
        }
        [HttpGet("AverageWaitingForToday")]
        public async Task<IActionResult> AverageWaitingForToday(DateTime date)
        {
            return await GetResponseHandler(async () => await service.AverageWaitingForToday(date));
        }
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            return await AddItemResponseHandler(async () => await service.GetAllCustomers());
        }
        [HttpPost("EditCustomer")]
        public async Task<IActionResult> EditCustomer(CustomerEditDTO model)
        {
            return await AddItemResponseHandler(async () => await service.EditCustomer(model));
        }

        [HttpGet("DeleteCustomerById/{id}")]
        public async Task<IActionResult> DeleteCustomerById(int id)
        {
            return await GetResponseHandler(async () => await service.DeleteCustomerById(id));
        }


    }
}
