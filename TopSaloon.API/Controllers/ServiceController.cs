using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.ServiceLayer;
namespace TopSalon.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseResultHandlerController<ServiceService>
    {
        public ServiceController(ServiceService _service) : base(_service)
        {
        }
        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService(AddServiceDTO model)
        {
            return await AddItemResponseHandler(async () => await service.CreateService(model));
        }
        [HttpGet("DeleteService/{ID}")]
        public async Task<IActionResult> DeleteService(string ID)
        {
            return await AddItemResponseHandler(async () => await service.Deleteservice(ID));
        }
        
        [HttpPut("EditService")]
        public async Task<IActionResult> EditService(ServiceModelDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditService(model));
        }
        [HttpGet("getAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
             return await GetResponseHandler(async () => await service.GetAllServices());
        }
        [HttpGet("getServicesById/{id}")]
        public async Task<IActionResult> GetServicesById(int id)
        {
            return await GetResponseHandler(async () => await service.GetServicesById(id));
        }
        [HttpPost("modifyservice")]
        public async Task<IActionResult> modifyservice(ModifyService modifyService)
        {
            return await GetResponseHandler(async () => await service.ModifyService(modifyService));
        }




    }
}

 