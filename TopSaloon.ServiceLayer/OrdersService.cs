using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TopSaloon.Repository.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.Collections.Immutable;

namespace TopSaloon.ServiceLayer
{
    public class OrdersService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private IMapper mapper;

        public OrdersService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<OrderService>>> GetOrderServicesViaOrderId(int orderId)
        {

            ApiResponse<List<OrderService>> result = new ApiResponse<List<OrderService>>();

            try
            {
                var orderServicesToFetch = await unitOfWork.OrderServicesManager.GetAsync(b => b.OrderId == orderId);
                var orderServices = orderServicesToFetch.ToList();
                if (orderServices != null)
                {            
                    result.Data = orderServices;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order services");
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

        public async Task<ApiResponse<CompleteOrderDTO>> GetCompleteOrderById(int id)
        {

            ApiResponse<CompleteOrderDTO> result = new ApiResponse<CompleteOrderDTO>();

            try
            {
                var completeOrder = await unitOfWork.CompleteOrdersManager.GetByIdAsync(id);

                if (completeOrder != null)
                {
                    result.Data = mapper.Map<CompleteOrderDTO>(completeOrder);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to retreive complete order !");
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

        public async Task<ApiResponse<bool>> SetOrderService(int orderId)
        {

            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var orderServices = await unitOfWork.OrderServicesManager.GetByIdAsync(orderId);
                
                if (orderServices != null)
                {
                    orderServices.IsConfirmed = true;

                    var isUpdated = await unitOfWork.OrderServicesManager.UpdateAsync(orderServices);
                    if (isUpdated)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating order service !");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

        //----------------------------- Cancel Order ----------------------------------------------//
        public async Task<ApiResponse<string>> CancelOrder(string orderId)
        {
            //Cancel Order: remove order from queue, set next order (if found) time to current time and adjust queue.
            ApiResponse<string> result = new ApiResponse<string>();

            try
            {
                var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == Int32.Parse(orderId), 0, 0, null, includeProperties: "OrderServices");
                Order OrderToUpdate = order.FirstOrDefault();

                if (OrderToUpdate != null)
                {

                    var customer = await unitOfWork.CustomersManager.GetByIdAsync(OrderToUpdate.CustomerId);
                    var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(OrderToUpdate.BarberQueueId) ;
                    var barber = await unitOfWork.BarbersManager.GetByIdAsync(barberQueue.BarberId);

                    CompleteOrder completeOrder = new CompleteOrder();

                    completeOrder.OrderTotalAmount = completeOrder.OrderTotalAmount - (completeOrder.OrderTotalAmount * (OrderToUpdate.DiscountRate / 100));

                    completeOrder.OrderServicesList = "";

                    completeOrder.BarberId = barber.Id;
                    completeOrder.OrderDateTime = OrderToUpdate.OrderDate;
                    completeOrder.OrderFinishTime = OrderToUpdate.FinishTime;
                    completeOrder.CustomerId = customer.Id;
                    completeOrder.CustomerNameEN = customer.Name;
                    completeOrder.CustomerNameAR = customer.Name;
                    completeOrder.BarberNameAR = barber.NameAR;
                    completeOrder.BarberNameEN = barber.NameEN;
                    completeOrder.CustomerWaitingTimeInMinutes = OrderToUpdate.WaitingTimeInMinutes;
                    completeOrder.Status = "Canceled";
                    completeOrder.TotalTimeSpent = OrderToUpdate.TotalServicesWaitingTime;
                    completeOrder.OrderTotalAmount = 0;

                    for (int i = 0; i < order.FirstOrDefault().OrderServices.Count; i++)
                    {
                        completeOrder.OrderTotalAmount += order.FirstOrDefault().OrderServices[i].Price;
                    }

                    for (int i = 0; i < order.FirstOrDefault().OrderServices.Count; i++)
                    {
                        completeOrder.OrderServicesList = completeOrder.OrderServicesList + order.FirstOrDefault().OrderServices[i].ServiceId + ",";


                    }

                    List<ServicesToRecord> GoogleSheetServiceList = new List<ServicesToRecord>();


                    for (int i = 0; i < order.FirstOrDefault().OrderServices.Count; i++)
                    {
                        ServicesToRecord GoogleSheetServiceItem = new ServicesToRecord();
                        GoogleSheetServiceItem.ServiceNameAR = order.FirstOrDefault().OrderServices[i].NameAR;
                        GoogleSheetServiceItem.ServiceNameEN = order.FirstOrDefault().OrderServices[i].NameEN;
                        GoogleSheetServiceItem.ServiceTime = order.FirstOrDefault().OrderServices[i].Time;
                        GoogleSheetServiceItem.ServicePrice = order.FirstOrDefault().OrderServices[i].Price;
                        GoogleSheetServiceItem.ServiceStatus = order.FirstOrDefault().OrderServices[i].IsConfirmed;
                        GoogleSheetServiceList.Add(GoogleSheetServiceItem);

                    }

                    var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    DateTimeOffset localServerTime = DateTimeOffset.Now;
                    DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);



                    OrderToRecord GoogleSheetOrder = new OrderToRecord();
                    GoogleSheetOrder.BarberNameAR = completeOrder.BarberNameAR;
                    GoogleSheetOrder.BarberNameEN = completeOrder.BarberNameEN;
                    GoogleSheetOrder.CustomerNameAR = completeOrder.CustomerNameAR;
                    GoogleSheetOrder.CustomerNameEN = completeOrder.CustomerNameEN;
                    GoogleSheetOrder.DiscountRate = order.FirstOrDefault().DiscountRate;
                    GoogleSheetOrder.OrderTotalAmount = completeOrder.OrderTotalAmount;
                    GoogleSheetOrder.OrderEndTime = localTime.DateTime;
                    GoogleSheetOrder.OrderStartTime = order.FirstOrDefault().OrderDate;
                    GoogleSheetOrder.DiscountPrice = 0;

                    GoogleSheetOrder.Services = GoogleSheetServiceList;
                    AddOrderToGoogleSheets(GoogleSheetOrder);




                    //Fetch barber queue from order.
                    var barberQueueToFetch = await unitOfWork.BarbersQueuesManager.GetAsync(q => q.Id == OrderToUpdate.BarberQueueId, includeProperties: "Orders");
                    var queue = barberQueueToFetch.FirstOrDefault();

                    if(queue != null) { 
                    //Fetch and remove order to remove from Queue order list
                        var orderToRemove = queue.Orders.Find(o => o.Id == Int32.Parse(orderId));
                        queue.Orders.Remove(orderToRemove);

                        for (int i = 0; i < queue.Orders.Count; i++) // Update upcoming orders
                        {
                            if(i ==0 )
                            {
                                queue.Orders[i].OrderDate = DateTime.Now;
                                queue.Orders[i].FinishTime = queue.Orders[i].OrderDate.Value.AddMinutes(Convert.ToDouble(queue.Orders[i].TotalServicesWaitingTime));
                            }
                            else 
                            {
                                queue.Orders[i].OrderDate = queue.Orders[i - 1].FinishTime;
                                queue.Orders[i].FinishTime = queue.Orders[i].OrderDate.Value.AddMinutes(Convert.ToDouble(queue.Orders[i].TotalServicesWaitingTime));
                            } 
                        }

                        await unitOfWork.SaveChangesAsync();
                        result.Data = "Order cancelled successfully.";
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Could not fetch barber queue");
                        return result;
                    }
                }
                else
                {
                    result.Data = "Error";
                    result.Succeeded = false;
                    result.Errors.Add("Could not fetch order service");
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

        //--------------------------------- Confirm Order Services ----------------------------------------//
        public async Task<ApiResponse<string>> ConfirmOrderServices(List<OrderServiceDTO> orderServices)
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                int iteratorErrorCounter = 0;
                for(int i=0; i<orderServices.Count; i++)
                {
                    var serviceToFetch = await unitOfWork.OrderServicesManager.GetAsync(b => b.Id == orderServices[i].Id);
                    var service = serviceToFetch.ToList().FirstOrDefault();
                    service.IsConfirmed = orderServices[i].IsConfirmed;
                    var updateRes = await unitOfWork.OrderServicesManager.UpdateAsync(service);
                    if (updateRes)
                    {
                    }
                    else
                    {
                        iteratorErrorCounter++;
                    }
                }
                if(iteratorErrorCounter > 0)
                {
                    result.Data = "Error updating one or more order services status !";
                    result.Succeeded = false;
                    return result;
                }
                else
                {
                    await unitOfWork.SaveChangesAsync();
                    var completionRes = await FinalizeOrder(orderServices[0].OrderId);
                    if (completionRes.Succeeded)
                    {

                        var queueUpdateRes = await SetQueueWaitingTimes();
                        if (queueUpdateRes.Data)
                        {
                            result.Data = "Order services status updates successfully.";
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Data = "Error adjusting Queue waiting time! ";
                            result.Succeeded = false;
                            return result;
                        }

                    }
                    else
                    {
                        result.Data = "Error finalizing order !";
                        result.Succeeded = true;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<ApiResponse<bool>> StartOrder(int orderId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                var orderToUpdate = await unitOfWork.OrdersManager.GetByIdAsync(orderId);


                if (orderToUpdate != null) {
                    orderToUpdate.OrderDate = DateTime.Now;
                    orderToUpdate.FinishTime = orderToUpdate.OrderDate.Value.AddMinutes(Convert.ToDouble(orderToUpdate.TotalServicesWaitingTime));

                    var queueToFetch = await unitOfWork.BarbersQueuesManager.GetAsync(q => q.Id == orderToUpdate.BarberQueueId, includeProperties: "Orders");
                    var queue = queueToFetch.FirstOrDefault();

                    if(queue != null) // queue found
                    {
                        if(queue.Orders.Count > 0) // queue orders found
                        {
                            for(int i=1; i< queue.Orders.Count; i++) // Update upcoming orders
                            {
                                queue.Orders[i].OrderDate = queue.Orders[i - 1].FinishTime;
                                queue.Orders[i].FinishTime = queue.Orders[i].OrderDate.Value.AddMinutes(Convert.ToDouble(queue.Orders[i].TotalServicesWaitingTime));
                            }                            
                        }
                        else
                        {
                            result.Errors.Add("Unable to fetch queue orders");
                            result.Data = false;
                            result.Succeeded = false;
                            return result;
                        }
                    }
                    else
                    {
                        result.Errors.Add("Unable to fetch queue");
                        result.Data = false;
                        result.Succeeded = false;
                        return result;
                    }
                    await unitOfWork.SaveChangesAsync();

                    result.Data = true;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to fetch order");
                    result.Data = false;
                    result.Succeeded = false;
                    return result;
                }
            }
            catch(Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
            //--------------------------------- Finalize Order ------------------------------------------------//
        public async Task<ApiResponse<string>> FinalizeOrder(int orderId)
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try 
            {
                var orderToFetch = await unitOfWork.OrdersManager.GetAsync(o => o.Id == orderId, includeProperties: "OrderServices");
                var order = orderToFetch.FirstOrDefault();
                var orderToFinalize = new Order();
                var orderToExcelExtract = order;
                float totalAmountToExtract = 0; 
 
                if (order != null)
                {
                    CompleteOrder completeOrder = new CompleteOrder();
                
                    completeOrder.OrderServicesList = "";
                    
                
                   orderToFinalize.OrderServices  = order.OrderServices.Where(o => o.IsConfirmed == true).ToList(); // Filter confirmed services.

                    //Fetch customer and barber from order.
                    var customer = await unitOfWork.CustomersManager.GetByIdAsync(order.CustomerId);
                    var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(order.BarberQueueId);
                    var barber = await unitOfWork.BarbersManager.GetByIdAsync(barberQueue.BarberId);

                    //Create complete Order

                    completeOrder.OrderTotalAmount = 0;
                   
                    for (int i = 0; i < orderToFinalize.OrderServices.Count; i++)
                    {
                        completeOrder.OrderTotalAmount += orderToFinalize.OrderServices[i].Price;
                    }

                    totalAmountToExtract =(float)completeOrder.OrderTotalAmount; 

                    completeOrder.OrderTotalAmount = completeOrder.OrderTotalAmount - (completeOrder.OrderTotalAmount * (order.DiscountRate / 100));
 
                    completeOrder.BarberId = barber.Id;
                    completeOrder.OrderDateTime = order.OrderDate;
                    completeOrder.OrderFinishTime = order.FinishTime;
                    completeOrder.CustomerId = customer.Id;
                    completeOrder.CustomerNameEN = customer.Name;
                    completeOrder.CustomerNameAR = customer.Name;
                    completeOrder.BarberNameAR = barber.NameAR;
                    completeOrder.BarberNameEN = barber.NameEN;
                    completeOrder.CustomerWaitingTimeInMinutes = order.WaitingTimeInMinutes;
                    completeOrder.Status = "Finalized";
                    completeOrder.TotalTimeSpent = order.TotalServicesWaitingTime;
      

                    //Fill complete order services list

                    List<OrderService> orderServicesHistory = new List<OrderService>();
                    orderServicesHistory = orderToFinalize.OrderServices;

                    List<ServicesToRecord> GoogleSheetServiceList = new List<ServicesToRecord>();
                   
                    for(int i=0; i<order.OrderServices.Count; i++)
                    {
                        completeOrder.OrderServicesList = completeOrder.OrderServicesList + order.OrderServices[i].ServiceId + ",";

                       
                    }

                    //construct the list of the excel 
                    for (int i = 0; i < orderToExcelExtract.OrderServices.Count; i++)
                    {
                        ServicesToRecord GoogleSheetServiceItem = new ServicesToRecord();
                        GoogleSheetServiceItem.ServiceNameAR = orderToExcelExtract.OrderServices[i].NameAR;
                        GoogleSheetServiceItem.ServiceNameEN = orderToExcelExtract.OrderServices[i].NameEN;
                        GoogleSheetServiceItem.ServiceTime = orderToExcelExtract.OrderServices[i].Time;
                        GoogleSheetServiceItem.ServicePrice = orderToExcelExtract.OrderServices[i].Price;
                        GoogleSheetServiceItem.ServiceStatus = orderToExcelExtract.OrderServices[i].IsConfirmed;
                        GoogleSheetServiceList.Add(GoogleSheetServiceItem);

                    }

                    //Create complete order
                    var completeOrderCreationResult = await unitOfWork.CompleteOrdersManager.CreateAsync(completeOrder);
                    barber.NumberOfCustomersHandled++; // Increase barber # of customers handled counter

                    await unitOfWork.OrdersManager.RemoveAsync(order); // Remove order record.

                    await unitOfWork.SaveChangesAsync();
                    // Update upcoming orders

                    var queueResult = await unitOfWork.BarbersQueuesManager.GetAsync(q => q.Id == order.BarberQueueId, includeProperties: "Orders");
                    var queue = queueResult.FirstOrDefault();

                    for (int i = 0; i < queue.Orders.Count; i++) 
                    {
                        if (i == 0)
                        {
                            queue.Orders[i].OrderDate = DateTime.Now;
                            queue.Orders[i].FinishTime = queue.Orders[i].OrderDate.Value.AddMinutes(Convert.ToDouble(queue.Orders[i].TotalServicesWaitingTime));
                        }
                        else
                        {
                            queue.Orders[i].OrderDate = queue.Orders[i - 1].FinishTime;
                            queue.Orders[i].FinishTime = queue.Orders[i].OrderDate.Value.AddMinutes(Convert.ToDouble(queue.Orders[i].TotalServicesWaitingTime));
                        }
                    }

                    await unitOfWork.SaveChangesAsync();



                    //Create order feedback
                    OrderFeedback orderFeedback = new OrderFeedback();
                    orderFeedback.IsSubmitted = false;
                    orderFeedback.CompleteOrderId = completeOrderCreationResult.Id;

                    var orderFeedbackCreationResult = await unitOfWork.OrderFeedBacksManager.CreateAsync(orderFeedback);
                    await unitOfWork.SaveChangesAsync();



                    //Create Orderfeedback questions

                    for (int i = 0; i < orderServicesHistory.Count; i++)
                    {
                        var serviceFeedbackQuestionsResult = await unitOfWork.ServiceFeedBackQuestionsManager.GetAsync(s => s.ServiceId == orderServicesHistory[i].ServiceId);
                        var serviceFeedbackQuestionsList = serviceFeedbackQuestionsResult.ToList();
                        for (int j = 0; j < serviceFeedbackQuestionsList.Count; j++)
                        {
                            OrderFeedbackQuestion orderFeedbackQuestionToCreate = new OrderFeedbackQuestion();
                            orderFeedbackQuestionToCreate.OrderFeedbackId = orderFeedbackCreationResult.Id;
                            orderFeedbackQuestionToCreate.QuestionAR = serviceFeedbackQuestionsList[j].QuestionAR;
                            orderFeedbackQuestionToCreate.QuestionEN = serviceFeedbackQuestionsList[j].QuestionEN;
                            orderFeedbackQuestionToCreate.Rating = 0;
                            var FeedbackQuestionCreationResult = await unitOfWork.OrderFeedBackQuestionsManager.CreateAsync(orderFeedbackQuestionToCreate);
                            await unitOfWork.SaveChangesAsync();
                        }
                    }

                    //gets the egypt time to get the final time 
                    var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    DateTimeOffset localServerTime = DateTimeOffset.Now;
                    DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                   


                    //var googleSheetsRecordResult = await AddOrderToGoogleSheets(completeOrder);
                    OrderToRecord GoogleSheetOrder = new OrderToRecord();
                    GoogleSheetOrder.BarberNameAR = completeOrder.BarberNameAR;
                    GoogleSheetOrder.BarberNameEN = completeOrder.BarberNameEN;
                    GoogleSheetOrder.CustomerNameAR = completeOrder.CustomerNameAR;
                    GoogleSheetOrder.CustomerNameEN = completeOrder.CustomerNameEN;
                    GoogleSheetOrder.DiscountRate = orderToExcelExtract.DiscountRate;
                    GoogleSheetOrder.OrderTotalAmount = totalAmountToExtract;
                    GoogleSheetOrder.OrderEndTime = localTime.DateTime;
                    GoogleSheetOrder.OrderStartTime = orderToExcelExtract.OrderDate;
                    GoogleSheetOrder.DiscountPrice = totalAmountToExtract - completeOrder.OrderTotalAmount; 
                    
                    GoogleSheetOrder.Services = GoogleSheetServiceList;
                    AddOrderToGoogleSheets(GoogleSheetOrder); // Save order history in google spreadsheet
                    result.Succeeded = true;
                    result.Data = "Finalized successfully";
                    return result;
                }
                else
                {
                    result.Errors.Add("Failed to fetch order");
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
        public    ApiResponse<object> AddOrderToGoogleSheets(OrderToRecord GoogleSheetOrder)
        {
            ApiResponse<object> result = new ApiResponse<object>();

            var gsh = new GoogleSheetsHelper(); // Initialize Google Sheets Helper
            
            gsh.CreateEntry(GoogleSheetOrder);  // Create New google sheet row record
            result.Succeeded = true;
            return result;
        }

        public async Task<ApiResponse<bool>> SetQueueWaitingTimes()
        {
            var result = new ApiResponse<bool>();
            try
            {
                var barberQueuesResult = await unitOfWork.BarbersQueuesManager.GetAsync(includeProperties: "Orders");
                List<BarberQueue> barberQueue = barberQueuesResult.ToList();
                TimeSpan? orderTimeDifference;
                DateTime newDate;
                if (barberQueue != null)
                {
                    int countErrors = 0;
                    //Iterate and set time per queue
                    for (int i = 0; i < barberQueue.Count; i++)
                    {
                        if (barberQueue[i].Orders.Count > 0)
                        { // validate orders count in queue
                            if (DateTime.Now > barberQueue[i].Orders[0].FinishTime) // Last order finish time in queue is passed.
                            {
                                newDate = DateTime.Now;
                                newDate = newDate.AddMinutes(5);
                                barberQueue[i].Orders[0].FinishTime = newDate;

                                for (int k = 1; k < barberQueue[i].Orders.Count; k++) // Update upcoming orders
                                {
                                    barberQueue[i].Orders[k].OrderDate = barberQueue[i].Orders[k - 1].FinishTime;
                                    barberQueue[i].Orders[k].FinishTime = barberQueue[i].Orders[k].OrderDate.Value.AddMinutes(Convert.ToDouble(barberQueue[i].Orders[k].TotalServicesWaitingTime));

                                }
                                orderTimeDifference = barberQueue[i].Orders[barberQueue[i].Orders.Count - 1].FinishTime - DateTime.Now;
                                barberQueue[i].QueueWaitingTime = Convert.ToInt32(orderTimeDifference.Value.TotalMinutes);
                                await unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                orderTimeDifference = barberQueue[i].Orders[barberQueue[i].Orders.Count - 1].FinishTime - DateTime.Now;
                                barberQueue[i].QueueWaitingTime = Convert.ToInt32(orderTimeDifference.Value.TotalMinutes);
                                await unitOfWork.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            barberQueue[i].QueueWaitingTime = 0;
                            barberQueue[i].QueueStatus = "idle";
                        }
                    }
                    if (countErrors > 0)
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error fetching Barber Queues!");
                        return result;
                    }
                    else
                    {
                        await unitOfWork.SaveChangesAsync();
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                }//End OF MAIN IF  . 
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching Barber Queues");
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

        public async Task<ApiResponse<List<CompleteOrderDTO>>> getAllCompleteOrderByDate(DateTime date)
        {
            var result = new ApiResponse<List<CompleteOrderDTO>>();

            try
            {
                var Complete = await unitOfWork.CompleteOrdersManager.GetAsync(a=>a.OrderDateTime.Value.Date == date.Date);

                List<CompleteOrder> completeorderlist = Complete.ToList();

                if (completeorderlist != null)
                {
                    result.Data = mapper.Map<List<CompleteOrderDTO>>(completeorderlist);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching complete orders !");
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
        public async Task<ApiResponse<int>> getAllCompleteOrderRows()
        {
            var result = new ApiResponse<int>();

            try
            {
                var Complete = await unitOfWork.CompleteOrdersManager.GetAsync();

                List<CompleteOrder> completeorderlist = Complete.ToList();
                int rowsToCount = 0; 

                if (completeorderlist != null)
                {
                    for (int i =0; i<completeorderlist.Count; i++ )
                    {
                        var ids = completeorderlist[i].OrderServicesList.Split(',');
                        rowsToCount += ids.Length; 
                    }
                    result.Data = rowsToCount; 
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error fetching complete orders !");
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
