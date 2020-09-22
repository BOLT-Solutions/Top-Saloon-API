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

                if (order != null)
                {
                    CompleteOrder completeOrder = new CompleteOrder();
                    completeOrder.OrderServicesList = "";
                    order.OrderServices = order.OrderServices.Where(o => o.IsConfirmed == true).ToList(); // Filter confirmed services.

                    //Fetch customer and barber from order.
                    var customer = await unitOfWork.CustomersManager.GetByIdAsync(order.CustomerId);
                    var barberQueue = await unitOfWork.BarbersQueuesManager.GetByIdAsync(order.BarberQueueId);
                    var barber = await unitOfWork.BarbersManager.GetByIdAsync(barberQueue.BarberId);

                    //Create complete Order
                    completeOrder.BarberId = barber.Id;
                    completeOrder.OrderDateTime = order.OrderDate;
                    completeOrder.OrderFinishTime = order.FinishTime;
                    completeOrder.OrderTotalAmount = order.OrderTotal;
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
                    orderServicesHistory = order.OrderServices;

                    for(int i=0; i<order.OrderServices.Count; i++)
                    {
                        completeOrder.OrderServicesList = completeOrder.OrderServicesList + order.OrderServices[i].ServiceId + ",";
                        
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

                    for(int i=0; i<orderServicesHistory.Count; i++)
                    {
                        var serviceToFetch = await unitOfWork.ServiceFeedBackQuestionsManager.GetAsync(s => s.ServiceId == orderServicesHistory[i].ServiceId);
                        var service = serviceToFetch.FirstOrDefault();

                        OrderFeedbackQuestion orderFeedbackQuestionToCreate = new OrderFeedbackQuestion();
                        orderFeedbackQuestionToCreate.OrderFeedbackId = orderFeedbackCreationResult.Id;
                        orderFeedbackQuestionToCreate.QuestionAR = service.QuestionAR;
                        orderFeedbackQuestionToCreate.QuestionEN = service.QuestionEN;
                        orderFeedbackQuestionToCreate.Rating = 0;

                        var FeedbackQuestionCreationResult = await unitOfWork.OrderFeedBackQuestionsManager.CreateAsync(orderFeedbackQuestionToCreate);
                        await unitOfWork.SaveChangesAsync();
                    }
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
        //public async Task<ApiResponse<string>> FinalizeOrder(int orderId)
        //    {
        //        ApiResponse<string> result = new ApiResponse<string>();

        //    try
        //    {
        //        var order = await unitOfWork.OrdersManager.GetAsync(b => b.Id == orderId, 0, 0, null, includeProperties: "OrderServices");

        //        Order OrderToUpdate = order.FirstOrDefault();

        //        CompleteOrder OrderHistory = new CompleteOrder();

        //        OrderHistory.OrderServicesList = "";

        //        if (OrderToUpdate != null)
        //        {
        //            var customer = await unitOfWork.CustomersManager.GetByIdAsync(OrderToUpdate.CustomerId);

        //            if (customer != null)
        //            {
        //                OrderToUpdate.Status = "Done";
        //                OrderHistory.OrderDateTime = OrderToUpdate.OrderDate;
        //                OrderHistory.OrderFinishTime = OrderToUpdate.FinishTime;
        //                OrderHistory.OrderTotalAmount = OrderToUpdate.OrderTotal;
        //                OrderHistory.CustomerWaitingTimeInMinutes = OrderToUpdate.WaitingTimeInMinutes;
        //                OrderHistory.Status = OrderToUpdate.Status;
        //                OrderHistory.CustomerId = customer.Id;

        //                for (int i = 0; i < OrderToUpdate.OrderServices.Count; i++)
        //                {
        //                    if (OrderToUpdate.OrderServices[i].IsConfirmed.Value == true )
        //                    {
        //                        OrderHistory.OrderServicesList = OrderHistory.OrderServicesList + OrderToUpdate.OrderServices[i].ServiceId + ","; 
        //                    }
        //                }

        //                var OrderUpdateResult = await unitOfWork.OrdersManager.UpdateAsync(OrderToUpdate);


        //                if (OrderUpdateResult)
        //                {
        //                    var barberQueue = await unitOfWork.BarbersQueuesManager.GetAsync(b => b.Id == OrderToUpdate.BarberQueueId, 0, 0, null, includeProperties: "Orders");

        //                    if (barberQueue != null)
        //                    {
        //                        BarberQueue QueueToUpdate = barberQueue.FirstOrDefault();

        //                        for (int i = 0; i < QueueToUpdate.Orders.Count; i++)
        //                        {
        //                            if (QueueToUpdate.Orders[i].Id == orderId)
        //                            {
        //                                QueueToUpdate.Orders.Remove(QueueToUpdate.Orders[i]);
        //                            }
        //                        }

        //                        if (QueueToUpdate.Orders.Count == 0)
        //                        {
        //                            QueueToUpdate.QueueStatus = "idle";
        //                        }
        //                        else //Re-Adjust Queue Finish Time For Remaining Orders
        //                        {

        //                            await SetQueueWaitingTimes();
        //                        }

        //                        OrderHistory.BarberId = QueueToUpdate.BarberId;
        //                        await unitOfWork.BarbersQueuesManager.UpdateAsync(QueueToUpdate);
        //                        var completeOrder = await unitOfWork.CompleteOrdersManager.CreateAsync(OrderHistory);
        //                        await unitOfWork.SaveChangesAsync();

        //                        if (completeOrder != null)
        //                        {

        //                            OrderFeedback orderFeedback = new OrderFeedback();
        //                            orderFeedback.CompleteOrderId = completeOrder.Id;
        //                            orderFeedback.IsSubmitted = false;
        //                            var createdOrderFeedback = await unitOfWork.OrderFeedBacksManager.CreateAsync(orderFeedback);
        //                            await unitOfWork.SaveChangesAsync();
        //                            OrderToUpdate.OrderServices = OrderToUpdate.OrderServices.Where(o => o.IsConfirmed == true).ToList();
        //                            for (int i = 0; i < OrderToUpdate.OrderServices.Count; i++)
        //                            {

        //                                    var ServiceQuestionsToFetch = await unitOfWork.ServiceFeedBackQuestionsManager.GetAsync(s => s.ServiceId == OrderToUpdate.OrderServices[i].ServiceId);
        //                                    var ServiceQuestions = ServiceQuestionsToFetch.FirstOrDefault();

        //                                OrderFeedbackQuestion orderFeedbackQuestion = new OrderFeedbackQuestion
        //                                {
        //                                    QuestionAR = ServiceQuestions.QuestionAR,
        //                                    QuestionEN = ServiceQuestions.QuestionEN,
        //                                    //OrderFeedbackId = createdOrderFeedback.Id,
        //                                    Rating = 0
        //                                };

        //                                await unitOfWork.OrderFeedBackQuestionsManager.CreateAsync(orderFeedbackQuestion);
        //                                await unitOfWork.SaveChangesAsync();

        //                            }

        //                            Barber barberToUpdate = await unitOfWork.BarbersManager.GetByIdAsync(completeOrder.BarberId);

        //                            barberToUpdate.NumberOfCustomersHandled++;

        //                            var barberUpdateResult = await unitOfWork.BarbersManager.UpdateAsync(barberToUpdate);

        //                            if(barberUpdateResult == true)
        //                            {
        //                                customer.TotalNumberOfVisits++;

        //                                await unitOfWork.SaveChangesAsync();

        //                                result.Data = "Order finalized successfully.";
        //                                result.Succeeded = true;
        //                                return result;
        //                            }
        //                            else
        //                            {
        //                                result.Data = "Error";
        //                                result.Errors.Add("Failed to update barber !");
        //                                return result;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            result.Data = "Error";
        //                            result.Errors.Add("Failed to fetch barber Queue !");
        //                            return result;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        result.Data = "Error.";
        //                        result.Errors.Add("Error Finalizing order !");
        //                        result.Succeeded = false;
        //                        return result;
        //                    }
        //                }
        //                else
        //                {
        //                    result.Data = "Error.";
        //                    result.Errors.Add("Error fetching customer details !");
        //                    result.Succeeded = false;
        //                    return result;

        //                }
        //            }
        //            else
        //            {
        //                result.Data = "Error";
        //                result.Succeeded = false;
        //                result.Errors.Add("Could not fetch order service");
        //                return result;
        //            }
        //        }
        //        else
        //        {

        //            result.Data = "Error finding order";
        //            result.Succeeded = true;
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Succeeded = false;
        //        result.Errors.Add(ex.Message);
        //        return result;
        //    }
        //    }

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
                var Complete = await unitOfWork.CompleteOrdersManager.GetAsync(a=>a.OrderDateTime==date);
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

    }
}
