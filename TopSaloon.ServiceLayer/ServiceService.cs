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
                var GetAllServices = await unitOfWork.ServicesManager.GetAsync();
                
                var ListService = GetAllServices.ToList();
                
                int maxValue = -1;
               
                for(int i=0; i< ListService.Count(); i++)
                {
                    if( ListService[i].position > maxValue)
                    {
                        maxValue = ListService[i].position;

                    }
                   
                }    
               
                Service newService = new Service();
                newService.NameAR = model.NameAR;
                newService.NameEN = model.NameEN;
                newService.AdminPath = model.AdminPath;
                newService.UserPath = model.UserPath;
                newService.Time = model.Time;
                newService.Price = model.Price;
                newService.position = maxValue+1; 

              


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
                else if (ServiceResult != null && ServiceResult.isDeleted == true)
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
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Service already exists !");
                    result.ErrorType = ErrorType.LogicalError;
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
        public async Task<ApiResponse<bool>> Deleteservice(string ServiceId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int serviceId = Int32.Parse(ServiceId);
                var service = await unitOfWork.ServicesManager.GetByIdAsync(serviceId);
                
                var GetList = await unitOfWork.ServicesManager.GetAsync(b => b.position > service.position && b.isDeleted == false);
              
                var serviceResult = await unitOfWork.ServicesManager.UpdateAsync(service);

                var listoFservices = GetList.ToList();


                if(listoFservices !=null)
                {
                    service.position = -1;
                    service.isDeleted = true;
                    for (int i = service.position+1; i <= listoFservices.Count()-1; i++ )
                    {
                        listoFservices[i].position = listoFservices[i].position - 1; 
                    }
                    var updateQuery = await unitOfWork.ServicesManager.UpdateAsync(service);
                    var ExecuteQuery = await unitOfWork.SaveChangesAsync();
                    if(ExecuteQuery)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else

                    {
                        result.Succeeded = false;
                        result.Errors.Add("cann Change list");

                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("cann not get list");
                   
                    return result;
                }
                
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                result.ErrorType = ErrorType.SystemError;
                return result;
            }
        }








        public async Task<ApiResponse<bool>> Deleteservice2(string ServiceId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int serviceId = Int32.Parse(ServiceId);
                var service = await unitOfWork.ServicesManager.GetByIdAsync(serviceId);
                service.isDeleted = true;
                service.position = -1;
                var serviceResult = await unitOfWork.ServicesManager.UpdateAsync(service);
                if (serviceResult == true)
                {
                    var res2 = await unitOfWork.SaveChangesAsync();
                    if (res2)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        var allServices = await unitOfWork.ServicesManager.GetAsync();// gets all services and order it
                        var deletedServices = await unitOfWork.ServicesManager.GetAsync(a => a.isDeleted == true);
                        int negativeCount = deletedServices.Count();
                        allServices = allServices.OrderBy(a => a.position);
                        List<Service> services = allServices.ToList();
                        int referencePosition = 0;
                        bool ZeroExist = false; 
                        for (int i = negativeCount; i <= services.Count-negativeCount; i++)
                        {
                            

                            if (services[i].position != referencePosition)
                            {
                                if (services[i].position == 0)
                                {
                                    services[i].position = referencePosition;
                                    ZeroExist = true;
                                }
                                else
                                {
                                    if (!ZeroExist)
                                    {
                                        services[i].position = referencePosition;
                                        ZeroExist = true;

                                    }
                                    else
                                    {
                                        services[i].position = referencePosition ;

                                    }

                                }
                                
                               
                                referencePosition++;
                                
                            }
                            else if (services[i].position == referencePosition)
                            {
                                if(services[i].position == 0 )
                                {
                                    ZeroExist = true;
                                }
                                referencePosition = services[i].position;
                                
                             }
                           


                        }
                         await unitOfWork.SaveChangesAsync(); 
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
                    service.position = model.Time;
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
                   var ser =  services.OrderBy(b => b.position);
                    result.Data = mapper.Map<List<ServiceDTO>>(ser.ToList());
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

        public async Task<ApiResponse<List<ServiceDTO>>> ModifyService(ModifyService modifyService)
        {
            ApiResponse<List<ServiceDTO>> result = new ApiResponse<List<ServiceDTO>>();
            try
            {
                var serviceToupdatePosition = await unitOfWork.ServicesManager.GetByIdAsync(modifyService.ServiceId); // gets service old position 
                var allServices = await unitOfWork.ServicesManager.GetAsync(a => a.isDeleted == false);  // gets all services and order it 
                allServices = allServices.OrderBy(a => a.position);
                List<Service> services = new List<Service>(); 
                if (serviceToupdatePosition != null)
                {
                     services = allServices.ToList();
                    // dragged down
                    if (modifyService.newPosition > modifyService.oldPosition)
                    {
                        services[modifyService.oldPosition].position = modifyService.newPosition;
                        // int serviceStopcondition = services.Count-(modifyService.newPosition - mod) ;


                        for (int i = modifyService.oldPosition + 1; i <= modifyService.newPosition; i++)
                        {
                            services[i].position = services[i].position - 1;

                        }

                    }
                    //draged up
                    else if (modifyService.newPosition < modifyService.oldPosition)
                    {
                        services[modifyService.oldPosition].position = modifyService.newPosition;


                        for (int i = modifyService.newPosition; i < modifyService.oldPosition; i++)
                        {


                            services[i].position = services[i].position + 1;




                        }
                    }
                
               //  var lol = mapper.Map<Service>(services); 
               //  var Saves =   await unitOfWork.ServicesManager.UpdateAsync(lol);
                var saved =  await unitOfWork.SaveChangesAsync();
                if(saved) 
                {
                            var GetLatest = await unitOfWork.ServicesManager.GetAsync();
                            var list = GetLatest.ToList();
                  if (list != null)
                            {
                                result.Succeeded = true;
                                result.Data = mapper.Map<List<ServiceDTO>>(list);
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Can not Get list of services ");
                                return result;
                            }
                            
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Can not save changes of Services ");
                            return result;
                        }
                    
                    
                  
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("cannot get any service ");
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
