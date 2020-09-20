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
using System.Reflection.Metadata.Ecma335;

namespace TopSaloon.ServiceLayer
{
    public class DailyReportService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public DailyReportService(UnitOfWork unitOfWork, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }


        public async Task<ApiResponse<DailyReport>> SaveDailyReport(DailyReportDTO dailyReport)
        {

            ApiResponse<DailyReport> result = new ApiResponse<DailyReport>();

            try
            {
                DailyReport report = new DailyReport();
                report.ReportDate = dailyReport.ReportDate;
                report.TotalAmountOfServicesCost = dailyReport.TotalAmountOfServicesCost;
                report.TotalNumberOfBarbersSignedIn = dailyReport.TotalNumberOfBarbersSignedIn;
                report.TotalNumberOfCustomers = dailyReport.TotalNumberOfCustomers;
                report.AverageCustomerWaitingTimeInMinutes = dailyReport.AverageCustomerWaitingTimeInMinutes;

                var CreatedReport = await unitOfWork.DailyReportsManager.CreateAsync(report);

                result.Data = report;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<int>> GetTotalNumberCustomerForToday()
        {
            
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A=>A.OrderDateTime.Value.Date >= myday );
                if (CO != null)
                {
                    result.Data = CO.Count();
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<int>> GetTotalServiceCostForToday()
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                string filter = "LastWeek"; 
                var myday = DateTime.Today ;
                int day = myday.Day;
                int month = myday.Month;
                int year = myday.Year;
                int lollll = DateTime.DaysInMonth(year, month - 1);
                // get the past date 
                if (filter=="LastWeek")
                { 

                }
               
                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime.Value.Date >= myday);
                if (CO != null)
                {
                    int total = 0;
                    var COList = CO.ToList(); 
                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total +=(int)COList[i].OrderTotalAmount;  
                    }
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<float>> GetAverageOfWaitingTimeForToday( )
        {

            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                //complete order object
                var myday = DateTime.Today;

                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime.Value.Date >= myday);
                if (CO != null)
                {
                    var COList = CO.ToList();
                    float total = 0;
                   
                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total += (float) COList[i].CustomerWaitingTimeInMinutes;
                    }
                    total = total / COList.Count(); 
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<int>> GetNumberOfSignedInBarbersForToday( )
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;

                var CO = await unitOfWork.DailyReportsManager.GetSigndInbarbers();

                if (CO != 0)
                {
                   
                    result.Data = CO;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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

        //GetTotalAmountOfCostPerDay
        public async Task<ApiResponse<List<CompleteOrder>>> GetTotalAmountOfCostPerDay()
        {

            ApiResponse<List<CompleteOrder>> result = new ApiResponse<List<CompleteOrder>>();

            try
            {

                var CO = await unitOfWork.DailyReportsManager.CosteachDay();
                List<CompleteOrder> CoList = CO.ToList();
                CoList = CoList.OrderByDescending(a => a.OrderDateTime).ToList(); 
                int x =  CO.GroupBy(a=>a.OrderDateTime).Distinct().Count();
                bool first = true;  
                int current = 0; 

                if (CO != null)
                {
                    List<CompleteOrder> Temp = new List<CompleteOrder>();
                    DateTime? currentholding = CoList[0].OrderDateTime;
                    
                    for (int i = 0; i < x; i++)
                    {
                        Temp.Add(CoList[current]);

                        for (int j = current; j < CoList.Count(); j++)
                        {
                           
                            if (currentholding.Value == CoList[j].OrderDateTime)
                            {
                                if (first)
                                {
                                    first = false; 
                                    continue; 
                                    
                                }
                                else
                                {
                                    Temp[i].OrderTotalAmount += CoList[j].OrderTotalAmount;
                                }
                                

                              
                            }
                            else
                            {
                                current = j;
                                first = true; 
                                break;  
                            }
                        }
                        currentholding =(DateTime)CoList[current].OrderDateTime; 
 
                    }

                    result.Data = Temp;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        /// per given date
        /// 
        public async Task<ApiResponse<int>> GetTotalNumberCustomer(string filter)
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var lastdate = CalcDate(filter);
                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime >= lastdate && A.OrderDateTime <= myday);
                if (CO != null)
                {
                    result.Data = CO.Count();
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<int>> GetTotalServiceCost(string filter)
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var lastdate = CalcDate(filter); 

                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime>=lastdate && A.OrderDateTime<=myday);
                if (CO != null)
                {
                    int total = 0;
                    var COList = CO.ToList();
                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total += (int)COList[i].OrderTotalAmount;
                    }
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<float>> GetAverageOfWaitingTime(string filter)
        {

            ApiResponse<float> result = new ApiResponse<float>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
               var  lastdate = CalcDate(filter);

                var CO = await unitOfWork.CompleteOrdersManager.GetAsync(A => A.OrderDateTime >= lastdate && A.OrderDateTime <= myday);
                if (CO != null)
                {
                    var COList = CO.ToList();
                    float total = 0;

                    for (int i = 0; i < CO.Count(); i++)
                    {
                        total += (float)COList[i].CustomerWaitingTimeInMinutes;
                    }
                    total = total / COList.Count();
                    result.Data = total;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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
        public async Task<ApiResponse<int>> GetNumberOfSignedInBarbers(string filter)
        {

            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                //complete order object
                var myday = DateTime.Today;
                var lastdate = CalcDate(filter); 

                var CO = await unitOfWork.DailyReportsManager.GetSigndInbarbers(lastdate);

                if (CO != 0)
                {

                    result.Data = CO;
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Could not Find Any Complete order");
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

        //helper
        private DateTime CalcDate(string filter )
        {
            var myday = DateTime.Today;
            int day = myday.Day;
            int month = myday.Month;
            int year = myday.Year;
            var lastdate = new DateTime();
            if (filter == "lastweek")
            {
                if (day > 7)
                {
                    lastdate = new DateTime(year, month, day - 7);
                }
                else
                {
                    int ndays = DateTime.DaysInMonth(year, month - 1) - day;
                    lastdate = new DateTime(year, month - 1, ndays);
                }

            }
            else if (filter == "lastmonth")
            {
                bool leap = false;
                leap = DateTime.IsLeapYear(year);
                if (month == 2)
                {
                    if (leap)
                    {
                        day = 29;
                    }
                    else { day = 28; }
                }
                lastdate = new DateTime(year, month - 1, day);

            }
            else if (filter == "lastyear")
            {
                lastdate = new DateTime(year - 1, month, day);

            }
            return lastdate; 
        }
    }
}

