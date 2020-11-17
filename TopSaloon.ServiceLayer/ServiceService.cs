using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;

namespace TopSaloon.ServiceLayer
{
   public class ServiceService
    {
        private readonly UnitOfWork unitOfWork;
         private readonly IConfiguration config;
        private readonly IMapper mapper;

        public ServiceService(UnitOfWork unitOfWork , IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.config = config;
        }
        public async Task<ApiResponse<bool>> CreateService(AddServiceDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Service newService = new Service();
                newService.NameAR = model.NameAR;
                newService.NameEN = model.NameEN;
                newService.AdminPath = model.AdminPath;
                newService.UserPath = model.UserPath;
                newService.Time = model.Time;
                newService.Price = model.Price;
                Service ServiceResult = await unitOfWork.ServicesManager.GetServiceByNameAR(model.NameAR);
                if (ServiceResult == null)
                {
                    var createServiceResult = await unitOfWork.ServicesManager.CreateAsync(newService);
                    await unitOfWork.SaveChangesAsync();
                    if (createServiceResult != null)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create Service !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Service already exists !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<bool>> Deleteservice(string ServiceId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int serviceId = Int32.Parse(ServiceId);
                var service = await unitOfWork.ServicesManager.GetByIdAsync(serviceId);
                service.isDeleted = true;
                var serviceResult = await unitOfWork.ServicesManager.UpdateAsync(service);
                if (serviceResult == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error Deleting Service !");
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Service doesn't exist !");
                result.ErrorType = ErrorType.LogicalError;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }
        public async Task<ApiResponse<ServiceDTO>> EditService( ServiceModelDTO model)
        {
            ApiResponse<ServiceDTO> result = new ApiResponse<ServiceDTO>();
            try
            {
                var service = await unitOfWork.ServicesManager.GetByIdAsync(model.Id);
                Service servEN = await unitOfWork.ServicesManager.GetServiceByNameEN(model.NameEN);
                Service servAR = await unitOfWork.ServicesManager.GetServiceByNameAR(model.NameAR);

                if ((model.NameAR==service.NameAR && model.NameEN==model.NameEN)||( servEN==null && servAR==null))
                {
                    service.NameAR = model.NameAR;
                    service.NameEN = model.NameEN;
                    service.Price = model.Price;
                    service.Time = model.Time;
                    service.AdminPath = model.AdminImagePath;
                    service.UserPath = model.UserImagePath;

                    await unitOfWork.ServicesManager.UpdateAsync(service);

                    var res = await unitOfWork.SaveChangesAsync();

                    if(res == true)
                    {
                        result.Data = mapper.Map<ServiceDTO>(service);
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating service !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }

                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("A service with a similar name alreadyd exists !");
                    result.ErrorType = ErrorType.LogicalError;
                    return result;
                }
                    
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
            }
            return result;
        }

        public async Task<ApiResponse<List<ServiceDTO>>> GetAllServices()
        {
            ApiResponse<List<ServiceDTO>> result = new ApiResponse<List<ServiceDTO>>();
            try
            {
               var services  = await unitOfWork.ServicesManager.GetAsync(b => b.isDeleted==false, includeProperties: "FeedBackQuestions");
                if (services != null)
                {
                    result.Data = mapper.Map<List<ServiceDTO>>(services.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to get list !");
                    result.Succeeded = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<ServiceDTO>> GetServicesById(int id)
        {
            ApiResponse<ServiceDTO> result = new ApiResponse<ServiceDTO>();
            try
            {
                var services = await unitOfWork.ServicesManager.GetByIdAsync(id);
                if (services != null)
                {
                    result.Data = mapper.Map<ServiceDTO>(services); 
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to get the service maybe wrong id!");
                    result.Succeeded = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

    }
}
