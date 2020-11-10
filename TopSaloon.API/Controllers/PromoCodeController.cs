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
    public class PromoCodeController : BaseResultHandlerController<PromoCodeService>
    {
        public PromoCodeController(PromoCodeService _service) : base(_service)
        {

        }

        [HttpPost("CreatePromoCode")]
        public async Task<IActionResult> CreatePromoCode(CreatePromoCodeDTO model)
        {
            return await AddItemResponseHandler(async () => await service.CreatePromoCode(model));
        }

        [HttpPost("DeletePromoCode")]
        public async Task<IActionResult> DeletePromoCode(DeletePromoCodeDTO model)
        {
            return await RemoveItemResponseHandler(async () => await service.DeletePromoCode(model));
        }

        [HttpPost("EditPromoCode")]
        public async Task<IActionResult> EditPromoCode(PromoCodeDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditPromoCode(model));
        }

        [HttpPost("ApplyPromoCode")]
        public async Task<IActionResult> ApplyPromoCode(ApplyPromoCodeDTO model)
        {
            return await GetResponseHandler(async () => await service.ApplyPromoCode(model));
        }

        [HttpGet("GetAllPromoCodes")]
        public async Task<IActionResult> GetAllPromoCodes()
        {
            return await GetResponseHandler(async () => await service.GetAllPromoCodes());
        }

    }
}
