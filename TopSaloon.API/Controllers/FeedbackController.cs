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
    public class FeedbackController : BaseResultHandlerController<FeedbackService>
    {
        public FeedbackController(FeedbackService _service) : base(_service)
        {

        }

        [HttpPost("AddServiceFeedbackQuestion")]
        public async Task<IActionResult> AddServiceFeedbackQuestion(AddServiceFeedbackQuestionDTO model)
        {
            return await AddItemResponseHandler(async () => await service.AddServiceFeedbackQuestion(model));
        }

        [HttpPost("RemoveServiceFeedbackQuestion")]
        public async Task<IActionResult> RemoveServiceFeedbackQuestion(string questionId)
        {
            return await AddItemResponseHandler(async () => await service.RemoveServiceFeedbackQuestion(questionId));
        }

        [HttpPost("EditServiceFeedbackQuestion")]
        public async Task<IActionResult> EditServiceFeedbackQuestion(EditServiceFeedbackQuestionDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditServiceFeedbackQuestion(model));
        }

        [HttpGet("GetServiceFeedBackQuestionsByServiceId")]
        public async Task<IActionResult> GetAllServiceFeedbackQuestionsByServiceId(string id)
        {
            return await GetResponseHandler(async () => await service.GetAllServiceFeedbackQuestionsByServiceId(id));
        }

    }
}