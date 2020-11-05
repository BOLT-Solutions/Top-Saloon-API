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
using AutoMapper;

namespace TopSaloon.ServiceLayer
{
    public class CustomerService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public CustomerService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }
        public async Task<ApiResponse<CustomerInfoDTO>> Login(CustomerLoginDTO loginRequest)
        {
            ApiResponse<CustomerInfoDTO> result = new ApiResponse<CustomerInfoDTO>();

            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetCustomerByPhoneNumber(loginRequest.MobileNumber);

                if (customer != null)
                {


                    if (customer.TotalNumberOfVisits == null)
                    {
                        customer.TotalNumberOfVisits = 1;
                        customer.LastVisitDate = DateTime.Now;
                    }
                    else
                    {
                        customer.TotalNumberOfVisits++;
                        customer.LastVisitDate = DateTime.Now;
                    }


                    await unitOfWork.SaveChangesAsync();

                    CustomerInfoDTO customerInfo = new CustomerInfoDTO();
                    customerInfo.Id = customer.Id;
                    customerInfo.Name = customer.Name;
                    customerInfo.LastBarberId = customer.LastBarberId;
                    customerInfo.LastVisitDate = customer.LastVisitDate;
                    customerInfo.TotalNumberOfVisits = customer.TotalNumberOfVisits;
                    customerInfo.PhoneNumber = customer.PhoneNumber;

                    result.Data = customerInfo;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Phone number not registered , Kindly create a new account !");
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
        public async Task<ApiResponse<CustomerDTO>> AddNewCustomer(AddCustomerModel model)
        {
            ApiResponse<CustomerDTO> result = new ApiResponse<CustomerDTO>();
            try
            {
                Customer newCustomer = new Customer();
                newCustomer.Name = model.Name;
                newCustomer.PhoneNumber = model.PhoneNumber;
                newCustomer.Email = model.Email;
                newCustomer.TotalNumberOfVisits = 0;

                Customer customerResult = await unitOfWork.CustomersManager.GetCustomerByPhoneNumber(model.PhoneNumber);

                if (customerResult == null)
                {
                    Customer createCustomerResult = await unitOfWork.CustomersManager.CreateAsync(newCustomer);

                    await unitOfWork.SaveChangesAsync();

                    if (createCustomerResult != null)
                    {
                        result.Data = mapper.Map<CustomerDTO>(createCustomerResult); ;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create customer !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                result.Succeeded = false;
                result.Errors.Add("Customer already exists !");
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
        public async Task<ApiResponse<int>> GetCustomerTotalNumberOfVisit(int UserId)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetByIdAsync(UserId);

                if (customer != null)
                {
                    result.Data = (int)customer.TotalNumberOfVisits;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It Is New Customer !");
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
        public async Task<ApiResponse<CustomerDTO>> GetCustomerVisitDetails(int CustomerId)
        {
            ApiResponse<CustomerDTO> result = new ApiResponse<CustomerDTO>();

            try
            {
                
                // CWO is Customer With there Orders 
                var CWO = await unitOfWork.CustomersManager.GetAsync(b => b.Id == CustomerId, includeProperties: "CompleteOrders");
                List<Customer> customer = CWO.ToList(); 
                if (CWO != null)
                {
                     
                    result.Data = mapper.Map<CustomerDTO>(customer[0]);
                    result.Succeeded = true;
                    return result;
                }   
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("There is NO Customer exist with This ID !");
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
        public async Task<ApiResponse<int>> GetNumberOfCustomerVisitForToday(DateTime date)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.TotalVisitperDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It Is New Customer !");
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
        public async Task<ApiResponse<float>> TotalAmountOfServiceCostForToday(DateTime date)
        {
            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.TotalAmountOfServiceCostPerDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It Is New Customer !");
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
        public async Task<ApiResponse<float>> AverageWaitingForToday(DateTime date)
        {
            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                var AllOrdersByDay = await unitOfWork.OrdersManager.AverageWaitingTimePerDay(date);

                if (AllOrdersByDay != 0)
                {
                    result.Data = AllOrdersByDay;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("No Total Time Calculated!");
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
        public async Task<ApiResponse<CustomerInfoDTO>> GuestLogin()
        {
            ApiResponse<CustomerInfoDTO> result = new ApiResponse<CustomerInfoDTO>();

            try
            {

                var currentGuestNumberResult = await unitOfWork.GuestNumberManager.GetAsync();

                List<GuestNumber> currentGuestNumberList = currentGuestNumberResult.ToList();

                if (currentGuestNumberList.Count == 0)
                {
                    GuestNumber initialGuestNumber = new GuestNumber();

                    initialGuestNumber.CurrentGuestNumber = 0;


                    var guestNumberResult = await unitOfWork.GuestNumberManager.CreateAsync(initialGuestNumber);

                    await unitOfWork.SaveChangesAsync();

                    if (guestNumberResult != null)
                    {

                        GuestNumber currentGuestNumber = guestNumberResult;

                        Customer customer = new Customer();

                        customer.Name = "Guest " + currentGuestNumber.CurrentGuestNumber.ToString();

                        customer.LastVisitDate = DateTime.Now;

                        customer.PhoneNumber = "00000000";

                        customer.Email = "Guest";

                        customer.TotalNumberOfVisits = 1;

                        var customerResult = await unitOfWork.CustomersManager.CreateAsync(customer);

                        var guestNumberUpdateResult = await unitOfWork.GuestNumberManager.GetByIdAsync(currentGuestNumber.Id);

                        guestNumberUpdateResult.CurrentGuestNumber++;

                        var res = await unitOfWork.SaveChangesAsync();

                        CustomerInfoDTO customerInfoToReturn = new CustomerInfoDTO();

                        customerInfoToReturn.Id = customer.Id;
                        customerInfoToReturn.Name = customer.Name;
                        customerInfoToReturn.LastVisitDate = customer.LastVisitDate;

                        if (res == true)
                        {
                            result.Data = customerInfoToReturn;
                            result.Data.Name = customerResult.Name;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed to login as guest !");
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }


                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to initialize guest number !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;

                    }

                }
                else
                {

                    GuestNumber currentGuestNumber = currentGuestNumberList[0];

                    Customer customer = new Customer();

                    customer.Name = "Guest " + currentGuestNumber.CurrentGuestNumber.ToString();

                    customer.LastVisitDate = DateTime.Now;

                    customer.PhoneNumber = "*******";

                    customer.Email = "*******";

                    customer.TotalNumberOfVisits = 1;

                    var customerResult = await unitOfWork.CustomersManager.CreateAsync(customer);

                    var guestNumberUpdateResult = await unitOfWork.GuestNumberManager.GetByIdAsync(currentGuestNumber.Id);

                    guestNumberUpdateResult.CurrentGuestNumber++;

                    var res = await unitOfWork.SaveChangesAsync();

                    CustomerInfoDTO customerInfoToReturn = new CustomerInfoDTO();

                    customerInfoToReturn.Id = customer.Id;
                    customerInfoToReturn.Name = customer.Name;
                    customerInfoToReturn.LastVisitDate = customer.LastVisitDate;

                    if (res == true)
                    {
                        result.Data = customerInfoToReturn;
                        result.Data.Name = customerResult.Name;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to login as guest !");
                        result.ErrorType = ErrorType.LogicalError;
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
        public async Task<ApiResponse<List<CustomerDTO>>> GetAllCustomers()
        {
            ApiResponse<List<CustomerDTO>> result = new ApiResponse<List<CustomerDTO>>();

            try
            {
                var customerList = await unitOfWork.CustomersManager.GetAsync(a=>a.Id!=0 , includeProperties: "CompleteOrders");

                List<Customer> customerListToReturn = customerList.ToList();

                for (int i = 0; i < customerListToReturn.Count; i++)
                {

                    for(int j = 0; j < customerListToReturn[i].CompleteOrders.Count; j++ )
                    {

                        customerListToReturn[i].CompleteOrders[j].OrderDateTime = customerListToReturn[i].CompleteOrders[j].OrderDateTime.Value.Date;
                    }
                }

                if (customerListToReturn != null)
                {
                    result.Data = mapper.Map<List<CustomerDTO>>(customerListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive customers list !");
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
        public async Task<ApiResponse<CustomerDTO>> GetCustomerById(int CustomerId)
        {
            ApiResponse<CustomerDTO> result = new ApiResponse<CustomerDTO>();

            try
            {
                var customer = await unitOfWork.CustomersManager.GetByIdAsync(CustomerId);

            
                if (customer != null)
                {
                    result.Data = mapper.Map<CustomerDTO>(customer);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to find customer with the referenced id !");
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
        public async Task<ApiResponse<bool>> EditCustomer( CustomerEditDTO model) 
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Customer customer = await unitOfWork.CustomersManager.GetByIdAsync(model.Id);
                var cust = await unitOfWork.CustomersManager.GetCustomerByPhoneNumber(model.PhoneNumber);


                if (cust == null)
                {
                    customer.Name = model.Name;
                    customer.PhoneNumber = model.PhoneNumber;
                    customer.Email = model.Email;

                    await unitOfWork.CustomersManager.UpdateAsync(customer);

                    var res = await unitOfWork.SaveChangesAsync();

                    if (res == true)
                    {

                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Error updating customer details !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                else
                {

                    if(cust.Id == customer.Id)
                    {
                        customer.Name = model.Name;
                        customer.PhoneNumber = model.PhoneNumber;
                        customer.Email = model.Email;

                        await unitOfWork.CustomersManager.UpdateAsync(customer);

                        var res = await unitOfWork.SaveChangesAsync();

                        if (res == true)
                        {

                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Error updating customer details !");
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("A customer with a similar number already exists !");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
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

        public async Task<ApiResponse<bool>> DeleteCustomerById(int id)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var customerToDelete = await unitOfWork.CustomersManager.GetByIdAsync(id);

                if (customerToDelete != null)
                {
                    var RemoveCustomerResult = await unitOfWork.CustomersManager.RemoveAsync(customerToDelete);

                    await unitOfWork.SaveChangesAsync();

                    if (RemoveCustomerResult == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        result.Errors.Add("Customer deleted successfully !");
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to delete customer !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to fetch customer with specified id !");
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

