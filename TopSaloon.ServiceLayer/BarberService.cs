using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.Internal;
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
using Microsoft.VisualBasic;

namespace TopSaloon.ServiceLayer
{
    public class BarberService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public BarberService(UnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }
        public async Task<ApiResponse<int>> GetNumberOfAvailableBarbers()
        {
            ApiResponse<int> result = new ApiResponse<int>();
            try
            {
                result.Data =  await unitOfWork.BarbersManager.GetNumberOfAvailableBarber();

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        public async Task<ApiResponse<int>> BarberTotalNumberOfHandledCustomer(int BarberId)
        {
            ApiResponse<int> result = new ApiResponse<int>();

            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(BarberId);

                if (barber != null)
                {
                    result.Data = (int)barber.NumberOfCustomersHandled;
                    result.Succeeded = true;
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
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
        public async Task<ApiResponse<BarberDTO>> GetBarberDetailsReports(int BarberId)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                var barber = await unitOfWork.BarbersManager.GetAsync(A=>A.Id==BarberId && A.isDeleted==false, includeProperties: "BarberQueue");
                 List<Barber> barberData = barber.ToList();

                if (barber != null)
                {
                    result.Data = mapper.Map<BarberDTO>(barberData[0]);
                    result.Succeeded = true;
                   

                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("It May Be A New Barber !");
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
        public async Task<ApiResponse<BarberDTO>> CreateBarber(CreateBarberDTO model)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {

                var barberPrintResult = await unitOfWork.BarbersManager.GetAsync(a => a.BarberFingerPrintId == model.BarberFingerprintId);

                var barber = barberPrintResult.FirstOrDefault();

                if (barber == null)
                {
                    var shops = await unitOfWork.ShopsManager.GetAsync();

                    Shop shop = shops.FirstOrDefault();
                    Barber barberToAdd = new Barber();
                    barberToAdd.NameAR = model.NameAR;
                    barberToAdd.NameEN = model.NameEN;
                    barberToAdd.ShopId = shop.Id;
                    barberToAdd.NumberOfCustomersHandled = 0;
                    barberToAdd.BarberFingerPrintId = model.BarberFingerprintId;
                    barberToAdd.Status = "Unavailable";
                    var barberResult = await unitOfWork.BarbersManager.CreateAsync(barberToAdd);

                    await unitOfWork.SaveChangesAsync();

                    if (barberResult != null)
                    {
                        BarberProfilePhoto barberProfilePhoto = new BarberProfilePhoto();
                        barberProfilePhoto.BarberId = barberResult.Id;
                        barberProfilePhoto.AdminPath = model.BarberProfilePhotoPathAdmin;
                        barberProfilePhoto.UserPath = model.BarberProfilePhotoPathUser;

                        var barberProfilePhotoResult = await unitOfWork.BarberProfilePhotosManager.CreateAsync(barberProfilePhoto);

                        await unitOfWork.SaveChangesAsync();

                        if (barberProfilePhotoResult != null)
                        {
                            BarberQueue barberQueue = new BarberQueue();

                            barberQueue.BarberId = barberResult.Id;

                            barberQueue.QueueStatus = "idle";

                            barberQueue.QueueWaitingTime = 0;

                            var barberQueueResult = await unitOfWork.BarbersQueuesManager.CreateAsync(barberQueue);

                            await unitOfWork.SaveChangesAsync();

                            if (barberQueueResult != null)
                            {

                                var barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Id == barberResult.Id, includeProperties: "BarberQueue,BarberProfilePhoto");

                                Barber barberToReturn = barbers.FirstOrDefault();

                                if (barberToReturn != null)
                                {
                                    result.Succeeded = true;
                                    result.Data = mapper.Map<BarberDTO>(barberToReturn);
                                    result.Errors.Add("Failed to create barber !");
                                    return result;
                                }
                                else
                                {
                                    result.Succeeded = false;
                                    result.Errors.Add("Error creating barber !");
                                    return result;
                                }
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Error creating barber queue !");
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed to create barber profile photo !");
                            return result;
                        }

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("A barber already exists with the specified finger print id !");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to create barber !");
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
        public async Task<ApiResponse<bool>> EditBarber(EditBarberDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Barber BarberToEdit = await unitOfWork.BarbersManager.GetByIdAsync(model.Id);

                if(BarberToEdit != null)
                {
                    BarberProfilePhoto barberProfilePhotoToEdit = await unitOfWork.BarberProfilePhotosManager.GetProfilePhotoByBarberId(BarberToEdit.Id);

                    if(barberProfilePhotoToEdit != null)
                    {
                        BarberToEdit.NameAR = model.NameAR;
                        BarberToEdit.NameEN = model.NameEN;
                        BarberToEdit.BarberFingerPrintId = model.BarberFingerprintId;

                        var barberPrintResult = await unitOfWork.BarbersManager.GetAsync(a => a.BarberFingerPrintId == model.BarberFingerprintId);

                        var barber = barberPrintResult.FirstOrDefault();

                        if (barber !=null)
                        {
                           result.Succeeded = false;
                            result.Errors.Add("Finger Print Id is used ! ");
                            return result;
                        }
                        else
                        {
                            barberProfilePhotoToEdit.AdminPath = model.BarberProfilePhotoPathAdmin;
                            barberProfilePhotoToEdit.UserPath = model.BarberProfilePhotoPathUser;



                            var barberResult = await unitOfWork.BarbersManager.UpdateAsync(BarberToEdit);

                            var barberProfilePhotoResult = await unitOfWork.BarberProfilePhotosManager.UpdateAsync(barberProfilePhotoToEdit);



                            if (barberResult == true && barberProfilePhotoResult == true)
                            {

                                await unitOfWork.SaveChangesAsync();

                                result.Succeeded = true;
                                result.Data = true;
                                return result;

                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Failed to update barber information ! ");
                                return result;
                            }
                        }

                       
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Unable to find barber profile photo with specified barber id ! ");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Unable to find barber with specified id ! ");
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAllBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {
                var barbersList = await unitOfWork.BarbersManager.GetAsync(a=> a.isDeleted == false,includeProperties: "BarberProfilePhoto");

                List <Barber> barberListToReturn = barbersList.ToList();


                if (barberListToReturn != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(barberListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive barbers list !");
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
        public async Task<ApiResponse<List<BarberDTO>>> GetAvailableBarbers()
        {
            ApiResponse<List<BarberDTO>> result = new ApiResponse<List<BarberDTO>>();

            try
            {

                var res = await SetQueueWaitingTimes();

                var Barbers = await unitOfWork.BarbersManager.GetAsync(b => b.Status == "Available" && b.isDeleted == false, includeProperties: "BarberProfilePhoto,BarberQueue");

                if (Barbers != null)
                {
                    result.Data = mapper.Map<List<BarberDTO>>(Barbers.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to fetch available barbers list !");
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
        public async Task<ApiResponse<bool>> DeleteBarberById(int id)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var barberToDelete = await unitOfWork.BarbersManager.GetByIdAsync(id);
                barberToDelete.isDeleted = true;
                if (barberToDelete != null)
                {
                    var RemoveBarberResult = await unitOfWork.BarbersManager.UpdateAsync(barberToDelete);

                    await unitOfWork.SaveChangesAsync();

                    if(RemoveBarberResult == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        result.Errors.Add("Barber deleted successfully !");
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to delete barber !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to fetch barber with specified id !");
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
        public async Task<ApiResponse<List<CompleteOrder>>> GetBarberAllCustomersHandled(int id)
        {
            ApiResponse<List<CompleteOrder>> result = new ApiResponse<List<CompleteOrder>>();

            try
            {
                var barbersList = await unitOfWork.CompleteOrdersManager.GetAsync(A=>A.BarberId==id);

                List<CompleteOrder> barberListToReturn = barbersList.ToList();
                for (int i = 0; i < barberListToReturn.Count; i++)
                {
                    barberListToReturn[i].OrderDateTime = barberListToReturn[i].OrderDateTime.Value.Date; 
                }


                if (barberListToReturn != null)
                {
                    result.Data = mapper.Map<List<CompleteOrder>>(barberListToReturn);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Errors.Add("Unable to retreive barbers list !");
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
        public async Task<ApiResponse<BarberDTO>> ChangeBarberStatus(CheckBarber checkBarber)
        {
            ApiResponse<BarberDTO> result = new ApiResponse<BarberDTO>();
            try
            {
                Barber barberToEdit = await unitOfWork.BarbersManager.GetByIdAsync(checkBarber.Id);

                if(barberToEdit != null)
                {
                    var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    DateTimeOffset localServerTime = DateTimeOffset.Now;
                    DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);

                    DateTime date = localTime.DateTime;
                    var barberLoginsResult = await unitOfWork.BarberLoginsManager.GetAsync(result => result.BarberId==checkBarber.Id && result.LoginDateTime.Value.Date ==checkBarber.Date.Value.Date);

                    BarberLogin login = barberLoginsResult.FirstOrDefault();


                    if (login != null)
                    {
                        if (barberToEdit.Status == "Available")
                        {
                            barberToEdit.Status = "Unavailable";
                        }
                        else
                        {
                            barberToEdit.Status = "Available";
                        }

                        var res = await unitOfWork.BarbersManager.UpdateAsync(barberToEdit);

                        await unitOfWork.SaveChangesAsync();

                        if (res == true)
                        {
                            result.Succeeded = true;
                            result.Data = mapper.Map<BarberDTO>(barberToEdit);
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed to update barber status !");
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("barber is not logged in !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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
        public async Task<ApiResponse<bool>> SignInBarberAdmin(BarberLoginRequestAdminDTO request)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(request.BarberId);

                if (barber != null)
                {
                    var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == request.BarberId && b.LoginDateTime.Value.Date == request.Time.Date);

                    if(barberLoginResult.FirstOrDefault() == null)
                    {
                        var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                        DateTimeOffset localServerTime = DateTimeOffset.Now;
                        DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                        request.Time = localTime.DateTime;

                        BarberLogin newLogin = new BarberLogin();

                        newLogin.BarberId = barber.Id;

                        newLogin.LoginDateTime = request.Time;

                        var res = await unitOfWork.BarberLoginsManager.CreateAsync(newLogin);

                        await unitOfWork.SaveChangesAsync();

                        if(res != null)
                        {
                            result.Succeeded = true;
                            result.Data = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Failed to sign in barber !");
                            return result;
                        }

                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Data = false;
                        result.Errors.Add("Barber already signed in today !");
                        return result;
                    }
       
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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
        public async Task<ApiResponse<bool>> SignOutBarberAdmin(BarberLogoutRequestAdminDTO request)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                Barber barber = await unitOfWork.BarbersManager.GetByIdAsync(request.BarberId);

                if (barber != null)
                {
                    var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == request.BarberId && b.LoginDateTime.Value.Date == request.Time.Date);

                    if (barberLoginResult.FirstOrDefault() == null)
                    {
                        result.Succeeded = false;
                        result.Data = false;
                        result.Errors.Add("Barber hasn't signed in today , Barber needs to sign in first in order to be able to sign out !");
                        return result;
                    }
                    else
                    {
                        BarberLogin barberLoginToEdit = barberLoginResult.FirstOrDefault();
                        var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                        DateTimeOffset localServerTime = DateTimeOffset.Now;
                        DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                        request.Time = localTime.DateTime;
                        barberLoginToEdit.logoutDateTime = request.Time;

                        barber.Status = "Unavailable";

                        var res = await unitOfWork.BarberLoginsManager.UpdateAsync(barberLoginToEdit);

                        await unitOfWork.SaveChangesAsync();

                        if(res == true)
                        {
                            result.Succeeded = true;
                            result.Data = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Failed to sign out barber !");
                            return result;
                        } 
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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
        public async Task<ApiResponse<bool>> BarberAttendanceBiometric(BarberAttendanceDTO request)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                var barberResult = await unitOfWork.BarbersManager.GetAsync(a => a.BarberFingerPrintId == request.BarberFingerprintId);

                Barber barber = barberResult.FirstOrDefault();

                if (barber != null)
                {

                    if (request.Method == 0) // Login Request . 
                    {

                        var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == barber.Id && b.LoginDateTime.Value.Date == request.CheckDate.Date);

                        if (barberLoginResult.FirstOrDefault() == null)
                        {

                            BarberLogin newLogin = new BarberLogin();

                            newLogin.BarberId = barber.Id;

                            newLogin.LoginDateTime = request.CheckDate;

                            var res = await unitOfWork.BarberLoginsManager.CreateAsync(newLogin);

                            await unitOfWork.SaveChangesAsync();

                            if (res != null)
                            {
                                result.Succeeded = true;
                                result.Data = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Data = false;
                                result.Errors.Add("Failed to sign in barber !");
                                return result;
                            }

                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Barber already signed in today !");
                            return result;
                        }

                    }
                    else // Logout Request . 
                    {
                        var barberLoginResult = await unitOfWork.BarberLoginsManager.GetAsync(b => b.BarberId == barber.Id && b.LoginDateTime.Value.Date == request.CheckDate.Date);

                        if (barberLoginResult.FirstOrDefault() == null)
                        {
                            result.Succeeded = false;
                            result.Data = false;
                            result.Errors.Add("Barber hasn't signed in today , Barber needs to sign in first in order to be able to sign out !");
                            return result;
                        }
                        else
                        {
                            BarberLogin barberLoginToEdit = barberLoginResult.FirstOrDefault();

                            barberLoginToEdit.logoutDateTime = request.CheckDate;

                            barber.Status = "Unavailable";

                            var res = await unitOfWork.BarberLoginsManager.UpdateAsync(barberLoginToEdit);

                            await unitOfWork.SaveChangesAsync();

                            if (res == true)
                            {
                                result.Succeeded = true;
                                result.Data = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Data = false;
                                result.Errors.Add("Failed to sign out barber !");
                                return result;
                            }

                        }
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find specified barber !");
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

    }
}

