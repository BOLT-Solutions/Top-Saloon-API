﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DTOs.Models;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.Entities.Models;
using AutoMapper;


namespace TopSaloon.ServiceLayer
{
    public class FeedbackService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;


        public FeedbackService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<ServiceFeedBackQuestion>> AddServiceFeedbackQuestion(AddServiceFeedbackQuestionDTO questionToAdd)
        {

            ApiResponse<ServiceFeedBackQuestion> result = new ApiResponse<ServiceFeedBackQuestion>();
            try
            {
                ServiceFeedBackQuestion serviceFeedBackQuestionToAdd = new ServiceFeedBackQuestion();
                serviceFeedBackQuestionToAdd.Question = questionToAdd.Question;
                serviceFeedBackQuestionToAdd.ServiceId = questionToAdd.ServiceId;
                serviceFeedBackQuestionToAdd = await unitOfWork.ServiceFeedBackQuestionsManager.CreateAsync(serviceFeedBackQuestionToAdd);

                var res = await unitOfWork.SaveChangesAsync();

                if (res == true)
                {
                    result.Data = serviceFeedBackQuestionToAdd;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error while adding service feedback question");
                    return result;
                }

                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

        public async Task<ApiResponse<bool>> RemoveServiceFeedbackQuestion(string questionId)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var serviceFeedbackQuestionToRemove = await unitOfWork.ServiceFeedBackQuestionsManager.GetByIdAsync(Int32.Parse(questionId));

                var res = await unitOfWork.ServiceFeedBackQuestionsManager.RemoveAsync(serviceFeedbackQuestionToRemove);

                if (res == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2 == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    result.Errors.Add("Error while updating database !");
                    return result;

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error while removing service feedback question !");
                    return result;
                }

                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

        public async Task<ApiResponse<bool>> EditServiceFeedbackQuestion(EditServiceFeedbackQuestionDTO questionToEdit)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                var serviceFeedbackQuestionToEdit = await unitOfWork.ServiceFeedBackQuestionsManager.GetByIdAsync(questionToEdit.Id);

                if (serviceFeedbackQuestionToEdit != null)
                {
                    serviceFeedbackQuestionToEdit.Question = questionToEdit.Question;
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2 == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    result.Errors.Add("Error while updating database !");
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find service feedback question !");
                    return result;
                }
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

        public async Task<ApiResponse<List<OrderFeedbackDTO>>> GetAllOrderFeedbackQuestions()
        {
            ApiResponse<List<OrderFeedbackDTO>> result = new ApiResponse<List<OrderFeedbackDTO>>();
            try
            {
                var services = await unitOfWork.OrderFeedBacksManager.GetAsync(b => b.IsSubmitted == true ,includeProperties: "OrderFeedbackQuestions");
                 if (services !=null)
                {
                    result.Data = mapper.Map<List<OrderFeedbackDTO>>(services.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find service feedback question !");
                    return result;
                }
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

        public async Task<ApiResponse<OrderFeedbackDTO>> GetOrderFeedbackQuestionsById(int Id)
        {
            ApiResponse<OrderFeedbackDTO> result = new ApiResponse<OrderFeedbackDTO>();
            try
            {
                var orderFeedback = await unitOfWork.OrderFeedBacksManager.GetAsync(b => b.Id == Id, includeProperties: "OrderFeedbackQuestions");
                List<OrderFeedback> order= orderFeedback.ToList(); 

                if (order != null)
                {
                    result.Data = mapper.Map<OrderFeedbackDTO>(order[0]);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find service feedback question !");
                    return result;
                }
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }

        public async Task<ApiResponse<DataItemsResult<OrderFeedback>>> GetOrderFeedbackById(string id)
        {
            ApiResponse<DataItemsResult<OrderFeedback>> result = new ApiResponse<DataItemsResult<OrderFeedback>>();

            result.Data = new DataItemsResult<OrderFeedback>();

            int OrderFeedbackId = Int32.Parse(id);

            try
            {
                IQueryable<OrderFeedback> res = await unitOfWork.OrderFeedBacksManager.GetAsync(c => c.Id == OrderFeedbackId , 0, 0,null,"OrderFeedbackQuestions");

                result.Data.Items = res.ToList();
                result.Succeeded = true;
                return result;

                //else
                //{
                //    result.Succeeded = false;
                //    result.Errors.Add("Unable to find order feedback !");
                //    return result;
                //}
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }


        }

        public async Task<ApiResponse<List<OrderFeedback>>> GetAllOrderFeedback()
        {
            ApiResponse<List<OrderFeedback>> result = new ApiResponse<List<OrderFeedback>>();
            try
            {
                List<OrderFeedback> orderFeedbackQuestionsListToSend = await unitOfWork.OrderFeedBacksManager.GetFeedbackBySubmittedStatus();

                if (orderFeedbackQuestionsListToSend != null)
                {
                    result.Data = orderFeedbackQuestionsListToSend;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find any order feedbacks !");
                    return result;
                }
                //End of try . 
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }



    }
}
