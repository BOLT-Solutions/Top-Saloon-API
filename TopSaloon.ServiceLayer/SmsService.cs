
using KannelSendingService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.Core.Managers;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.ServiceLayer;

namespace TopSaloon.ServiceLayer
{
    public class SmsService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public SmsService(UnitOfWork unitOfWork,IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }
        public async Task<ApiResponse<String>> GetSMS()
        {
            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var smsResult = await unitOfWork.SMSManager.GetAsync();

                List<SMS> smsList = smsResult.ToList();

                if (smsList.Count == 0)
                {
                    SMS newSMS = new SMS();

                    newSMS.Body = "Default";


                    var res = await unitOfWork.SMSManager.CreateAsync(newSMS);

                    await unitOfWork.SaveChangesAsync();

                    if (res != null)
                    {
                        result.Data = newSMS.Body;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to save initial SMS !");
                        return result;
                    }

                }    
                else
                {
                    result.Succeeded = true;
                    result.Data = smsList[0].Body;
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
        public async Task<ApiResponse<bool>> EditSMS(SmsDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var smsResult = await unitOfWork.SMSManager.GetAsync();

                List<SMS> smsList = smsResult.ToList();

                SMS smsToUpdate = smsList[0];

                smsToUpdate.Body = model.Body;

                var res = await unitOfWork.SMSManager.UpdateAsync(smsToUpdate);

                await unitOfWork.SaveChangesAsync();

                if (res == true)
                {
                    result.Data = true;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to edit SMS !");
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
        
        public async Task<ApiResponse<int>> SendSMS(SendSMSDTO  sendSMS)
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                var binding = new BasicHttpBinding(); //Communicate with asp.net web service
                var endpoint = new EndpointAddress(new Uri("http://smsvas.vlserv.com/KannelSending/service.asmx")); // to locate and find what api would like to call

                if(endpoint != null)// check if no end point address with that link
                {
                    var channelFactory = new ServiceSoapClient(binding, endpoint); // call servicesoapClient (you should find it with KannelSendingService / reference.cs)
                    if (channelFactory !=null)
                    {

                        var GetMsg = await unitOfWork.SMSManager.GetAsync();
                        var SingleSMS = GetMsg.FirstOrDefault();
                        if(SingleSMS != null)
                        {
                            // call method you want with Parameters 
                            var feedbackLink = "CustomeLink.com";
                            var SMSFullText = SingleSMS.Body + " " + feedbackLink+"?orderId="+sendSMS.orderId;   

                            var serviceClient = await channelFactory.SendSMSAsync("Topsalon", "999Bh0JkC9", SMSFullText, "e", "Top S Elite", sendSMS.CustomerNumber);
                            if (serviceClient != null)
                            {
                                result.Data = serviceClient; //assign data with result 
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Can not Get Result of CheckCredit ");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Can not Get Msg ");
                            return result;
                        }
                        
                       
                       
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Can not bind Endpoint with http Binding ");
                        return result;
                    }
                    
                }
                else
                {
                    
                    result.Succeeded = false;
                    result.Errors.Add("Can not Get End Point ");
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
        public async Task<ApiResponse<int>> CheckCredit()
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                var binding = new BasicHttpBinding(); //Communicate with asp.net web service
                var endpoint = new EndpointAddress(new Uri("http://smsvas.vlserv.com/KannelSending/service.asmx")); // to locate and find what api would like to call

                if (endpoint != null)// check if no end point address with that link
                {
                    var channelFactory = new ServiceSoapClient(binding, endpoint); // call servicesoapClient (you should find it with KannelSendingService / reference.cs)
                    if (channelFactory != null)
                    {

                       
                            // call method you want with Parameters 
                           
                            var serviceClient = await channelFactory.CheckCreditAsync("Topsalon", "999Bh0JkC9");
                            if (serviceClient != null)
                            {
                                result.Data = serviceClient; //assign data with result 
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Can not Get Result of CheckCredit ");
                                return result;
                            }
                        
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Can not bind Endpoint with http Binding ");
                        return result;
                    }

                }
                else
                {

                    result.Succeeded = false;
                    result.Errors.Add("Can not Get End Point ");
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

        public async Task<ApiResponse<int>> SendSMSWithDLR(SendSMSDTO sendSMS)
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                var binding = new BasicHttpBinding(); //Communicate with asp.net web service
                var endpoint = new EndpointAddress(new Uri("http://smsvas.vlserv.com/KannelSending/service.asmx")); // to locate and find what api would like to call

                if (endpoint != null)// check if no end point address with that link
                {
                    var channelFactory = new ServiceSoapClient(binding, endpoint); // call servicesoapClient (you should find it with KannelSendingService / reference.cs)
                    if (channelFactory != null)
                    {

                        var GetMsg = await unitOfWork.SMSManager.GetAsync();
                        var SingleSMS = GetMsg.FirstOrDefault();
                        if (SingleSMS != null)
                        {
                            // call method you want with Parameters 
                            var feedbackLink = "CustomeLink.com";
                            var SMSFullText = SingleSMS.Body + " " + feedbackLink + "?orderId=" + sendSMS.orderId;

                            var serviceClient = await channelFactory.SendSMSWithDLRAsync("Topsalon", "999Bh0JkC9", SMSFullText, "e", "Top S Elite", sendSMS.CustomerNumber);
                            if (serviceClient != null)
                            {
                                result.Data = serviceClient; //assign data with result 
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Can not Get Result of CheckCredit ");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Can not Get Msg ");
                            return result;
                        }



                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Can not bind Endpoint with http Binding ");
                        return result;
                    }

                }
                else
                {

                    result.Succeeded = false;
                    result.Errors.Add("Can not Get End Point ");
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


