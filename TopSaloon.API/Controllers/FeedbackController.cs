﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSaloon.API.Controllers.Common;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
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

        [HttpGet("RemoveServiceFeedbackQuestion/{questionId}")]
        public async Task<IActionResult> RemoveServiceFeedbackQuestion(string questionId)
        {
            return await AddItemResponseHandler(async () => await service.RemoveServiceFeedbackQuestion(questionId));
        }

        [HttpPost("EditServiceFeedbackQuestion")]
        public async Task<IActionResult> EditServiceFeedbackQuestion(EditServiceFeedbackQuestionDTO model)
        {
            return await EditItemResponseHandler(async () => await service.EditServiceFeedbackQuestion(model));
        }

        [HttpGet("GetAllOrderFeedbackQuestions")]
        public async Task<IActionResult> GetAllOrderFeedbackQuestions()
        {
            return await GetResponseHandler(async () => await service.GetAllOrderFeedbackQuestions());
        }
         [HttpGet("GetOrderFeedbackQuestionsByOrderId/{Id}")]
        public async Task<IActionResult> GetOrderFeedbackQuestionsByOrderId(int Id)
        {
            return await GetResponseHandler(async () => await service.GetOrderFeedbackQuestionsByOrderId(Id));
        }
        [HttpGet("GetOrderFeedbackQuestionsByServiceId/{Id}")]
        public async Task<IActionResult> GetOrderFeedbackQuestionsByServiceId(int Id)
        {
            return await GetResponseHandler(async () => await service.GetOrderFeedbackQuestionsByServiceId(Id));
        }

        [HttpGet("GetFeedbackById")]
        public async Task<IActionResult> GetFeedbackById(string id)
        {
            return await GetResponseHandler(async () => await service.GetOrderFeedbackById(id));
        }
        [HttpGet("GetAllOrderFeedbacks")]
        public async Task<IActionResult> GetAllOrderFeedback()
        {
            return await GetResponseHandler(async () => await service.GetAllOrderFeedback());
        }
           [HttpPost("updateOrderFeedbackQuestion")]
        public async Task<IActionResult> updateOrderFeedbackQuestion(OrderFeedbackDTO orderFeedback)
        {
            return await AddItemResponseHandler(async () => await service.updateOrderFeedbackQuestion(orderFeedback));
        }


    }
}
