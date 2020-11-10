
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DTOs.Models;
using TopSaloon.Core;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
 using TopSaloon.Entities.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TopSaloon.ServiceLayer
{
    public class UsersService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public UsersService(UnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.config = config;
        }
        public async Task<ApiResponse<bool>> CreateRole(string roleName)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                bool x = await unitOfWork.RoleManager.RoleExistsAsync(roleName);
                if (!x)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;

                    var res = await unitOfWork.RoleManager.CreateAsync(role);

                    if (res.Succeeded)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    foreach (var error in res.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Unable to create role !");
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }

        }
        public async Task<ApiResponse<bool>> AssignRole(string userId, string roleName)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();

            try
            {
                var user = await unitOfWork.UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, roleName);

                    if (roleresult.Succeeded)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    result.Succeeded = false;
                    foreach (var error in roleresult.Errors)
                    {
                        result.Errors.Add(error.Description);
                    }
                    return result;
                }
                result.Succeeded = false;
                result.Errors.Add("Unable to find user !");
                return result;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Errors.Add(ex.Message);
                return result;
            }
        }
        public async Task<ApiResponse<bool>> CreateAdminAccount(AdminCreationModel model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var shopResult = await unitOfWork.ShopsManager.GetAsync();

                Shop shop = shopResult.FirstOrDefault();

                if(shop != null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.FirstName + model.LastName
                    };

                    var res = await unitOfWork.UserManager.CreateAsync(user, model.Password);

                    await unitOfWork.SaveChangesAsync();

                    if (res.Succeeded)
                    {

                        var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, "Administrator");

                        Administrator adminToCreate = new Administrator();
                        adminToCreate.UserId = user.Id;
                        adminToCreate.ShopId = shop.Id;
                        adminToCreate.Role = "Administrator";

                        var admin = await unitOfWork.AdministratorsManager.CreateAsync(adminToCreate);

                        var res2 = await unitOfWork.SaveChangesAsync();

                        if (res2 == true)
                        {
                            result.Data = true;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed To Create Adminstrator");
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        foreach (var error in res.Errors)
                        {
                            result.Errors.Add(error.Description);
                        }
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }    
                }
                else
                {

                    Shop newShop = new Shop();

                    newShop.Address = "Mars";

                    var CreateShopResult = await unitOfWork.ShopsManager.CreateAsync(newShop);

                    await unitOfWork.SaveChangesAsync();

                    if(CreateShopResult != null)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            UserName = model.FirstName + model.LastName
                        };

                        var res = await unitOfWork.UserManager.CreateAsync(user, model.Password);

                        await unitOfWork.SaveChangesAsync();

                        if (res.Succeeded)
                        {

                            var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, "Administrator");

                            Administrator adminToCreate = new Administrator();
                            adminToCreate.UserId = user.Id;
                            adminToCreate.ShopId = CreateShopResult.Id;
                            adminToCreate.Role = "Administrator";

                            var admin = await unitOfWork.AdministratorsManager.CreateAsync(adminToCreate);

                            var res2 = await unitOfWork.SaveChangesAsync();

                            if (res2 == true)
                            {
                                result.Data = true;
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Failed To Create Adminstrator");
                                result.ErrorType = ErrorType.LogicalError;
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            foreach (var error in res.Errors)
                            {
                                result.Errors.Add(error.Description);
                            }
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create shop !");
                        result.ErrorType = ErrorType.SystemError;
                        return result;
                    }

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
        public async Task<ApiResponse<bool>> CreateSuperAdminAccount(AdminCreationModel model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {

                var shopResult = await unitOfWork.ShopsManager.GetAsync();

                Shop shop = shopResult.FirstOrDefault();

                if (shop != null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.FirstName + model.LastName
                    };

                    var res = await unitOfWork.UserManager.CreateAsync(user, model.Password);

                    await unitOfWork.SaveChangesAsync();

                    if (res.Succeeded)
                    {

                        var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, "SuperAdmin");

                        Administrator adminToCreate = new Administrator();
                        adminToCreate.UserId = user.Id;
                        adminToCreate.ShopId = shop.Id;
                        adminToCreate.Role = "SuperAdmin";

                        var admin = await unitOfWork.AdministratorsManager.CreateAsync(adminToCreate);

                        var res2 = await unitOfWork.SaveChangesAsync();

                        if (res2 == true)
                        {
                            result.Data = true;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Failed To Create Super Adminstrator");
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        foreach (var error in res.Errors)
                        {
                            result.Errors.Add(error.Description);
                        }
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                else
                {

                    Shop newShop = new Shop();

                    newShop.Address = "Mars";

                    var CreateShopResult = await unitOfWork.ShopsManager.CreateAsync(newShop);

                    await unitOfWork.SaveChangesAsync();

                    if (CreateShopResult != null)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            UserName = model.FirstName + model.LastName
                        };

                        var res = await unitOfWork.UserManager.CreateAsync(user, model.Password);

                        await unitOfWork.SaveChangesAsync();

                        if (res.Succeeded)
                        {

                            var roleresult = await unitOfWork.UserManager.AddToRoleAsync(user, "SuperAdministrator");

                            Administrator adminToCreate = new Administrator();
                            adminToCreate.UserId = user.Id;
                            adminToCreate.ShopId = CreateShopResult.Id;
                            adminToCreate.Role = "SuperAdministrator";

                            var admin = await unitOfWork.AdministratorsManager.CreateAsync(adminToCreate);

                            var res2 = await unitOfWork.SaveChangesAsync();

                            if (res2 == true)
                            {
                                result.Data = true;
                                result.Succeeded = true;
                                return result;
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("Failed To Create Super Adminstrator");
                                result.ErrorType = ErrorType.LogicalError;
                                return result;
                            }
                        }
                        else
                        {
                            result.Succeeded = false;
                            foreach (var error in res.Errors)
                            {
                                result.Errors.Add(error.Description);
                            }
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to create shop !");
                        result.ErrorType = ErrorType.SystemError;
                        return result;
                    }

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

        public async Task<ApiResponse<bool>> DeleteAdmin(string AdminId)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                int adminId = Int32.Parse(AdminId);
                var admin = await unitOfWork.AdministratorsManager.GetByIdAsync(adminId);
                var user = await unitOfWork.UserManager.FindByIdAsync(admin.UserId);
                var rolesForUser = await unitOfWork.UserManager.GetRolesAsync(user);

                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var res = await unitOfWork.UserManager.RemoveFromRoleAsync(user, "Administrator");
                }

                var userResult = await unitOfWork.UserManager.DeleteAsync(user);
                var adminResult = await unitOfWork.AdministratorsManager.RemoveAsync(admin);

                if (adminResult == true && userResult.Succeeded == true)
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
                        result.Errors.Add("Error Deleting Administrator !");
                        return result;
                    }
                }
                result.Succeeded = false;
                foreach (var error in userResult.Errors)
                {
                    result.Errors.Add(error.Description);
                }
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
        public async Task<ApiResponse<AdministratorDTO>> LoginAsync(AdminLoginModel model)
        {
            ApiResponse<AdministratorDTO> result = new ApiResponse<AdministratorDTO>();
            try
            {
                var user = await unitOfWork.UserManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    bool res = await unitOfWork.UserManager.CheckPasswordAsync(user, model.Password);

                    if (res)
                    {
                        var admin = unitOfWork.AdministratorsManager.GetAdminByUserId(user.Id);

                        if(admin != null)
                        {

                            result.Data = admin;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("Cannot find an administrator with the specified id !");
                            result.ErrorType = ErrorType.LogicalError;
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Invalid login attempt.");
                        result.ErrorType = ErrorType.LogicalError;
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid login attempt.");
                    result.ErrorType = ErrorType.LogicalError;
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

        public async Task<ApiResponse<float>> GetUserDailyEarningPerTime(DateTime Start, DateTime End)
        {
            ApiResponse<float> result = new ApiResponse<float>();
            try
            {
                float Total = await unitOfWork.OrdersManager.GetUserDailyEarning(Start, End);
                if (Total != 0f)
                {
                    result.Data = Total;
                    result.Succeeded = true;
                    return result;

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Invalid login attempt.");
                    result.ErrorType = ErrorType.LogicalError;
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
        public async Task<ApiResponse<AdminCreationModel>> getAdminById(int adminId)
        {
            ApiResponse<AdminCreationModel> result = new ApiResponse<AdminCreationModel>();
            try
            {
                Administrator adminValue = await unitOfWork.AdministratorsManager.GetByIdAsync(adminId);


                if (adminValue != null)
                {

                    var adminData = await unitOfWork.UserManager.FindByIdAsync(adminValue.UserId);

                        if (adminData != null)
                        {
                            AdministratorDTO adminDto = new AdministratorDTO();
                            adminDto.Id = adminValue.Id;
                            adminDto.UserId = adminValue.UserId;
                            adminDto.ShopId = adminValue.ShopId;


                            AdminCreationModel adminModel = new AdminCreationModel();

                            adminModel.FirstName = adminData.FirstName;
                            adminModel.LastName = adminData.LastName;
                            adminModel.Email = adminData.Email;
                            adminModel.PhoneNumber = adminData.PhoneNumber;
                          

                            result.Data = adminModel;
                            result.Succeeded = true;
                            return result;
                        }
                        else
                        {
                            result.Succeeded = false;
                            result.Errors.Add("User not found");
                            return result;
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Invalid input value");
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
        public async Task<ApiResponse<AdminCreationModel>> getSubAdminByRoleName(string RoleName)
        {
            ApiResponse<AdminCreationModel> result = new ApiResponse<AdminCreationModel>();
            try
            {
                
                var getadmin = await unitOfWork.AdministratorsManager.GetAsync(b => b.Role == RoleName);

                var getfirstadmin = getadmin.FirstOrDefault();

                if(getfirstadmin !=null)
                {
                    var userData = await unitOfWork.UserManager.FindByIdAsync(getfirstadmin.UserId);
                    if (userData != null)
                    {
                        AdministratorDTO adminDto = new AdministratorDTO();
                        adminDto.Id = getfirstadmin.Id;
                        adminDto.UserId = getfirstadmin.UserId;
                        adminDto.ShopId = getfirstadmin.ShopId;
                        adminDto.Role = getfirstadmin.Role;


                        AdminCreationModel adminModel = new AdminCreationModel();

                        adminModel.FirstName = userData.FirstName;
                        adminModel.LastName = userData.LastName;
                        adminModel.Email = userData.Email;
                        adminModel.PhoneNumber = userData.PhoneNumber;


                        result.Data = adminModel;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("User not found");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("cannot get user ");
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
        public async Task<ApiResponse<bool>> EditAdminById(editAdministrator adminDto)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            {
                Administrator adminValue = await unitOfWork.AdministratorsManager.GetByIdAsync(adminDto.id);
                if (adminValue != null)
                {
                    var userdata = await unitOfWork.UserManager.FindByIdAsync(adminValue.UserId);
                    userdata.FirstName = adminDto.FirstName;
                    userdata.LastName = adminDto.LastName;
                    userdata.Email = adminDto.Email;
                    userdata.PhoneNumber = adminDto.PhoneNumber;
                    if (adminDto.password == null)
                    {
                        adminDto.password = userdata.PasswordHash;
                    }
                    else if (userdata.PasswordHash != adminDto.password)
                    {
                        var checkPassword = await unitOfWork.UserManager.CheckPasswordAsync(userdata, adminDto.password);
                        if (adminDto.newpassword == null)
                        {
                            result.Succeeded = false;
                            result.Errors.Add("password is null");
                            return result;
                        }
                        else
                        {
                            if (checkPassword)
                            {
                                var changePassword = await unitOfWork.UserManager.ChangePasswordAsync(userdata, adminDto.password, adminDto.newpassword);
                                if (changePassword != null)
                                {
                                    var hasher = new PasswordHasher<ApplicationUser>();
                                    userdata.PasswordHash = hasher.HashPassword(userdata, adminDto.newpassword);
                                }
                                else
                                {
                                    result.Succeeded = false;
                                    result.Errors.Add("change not true");
                                    return result;
                                }
                            }
                            else
                            {
                                result.Succeeded = false;
                                result.Errors.Add("check password not true");
                                return result;
                            }
                        }
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add(" not true");
                        return result;
                    }
                    var res = await unitOfWork.UserManager.UpdateAsync(userdata);
                    if (res.Succeeded)
                    {
                        await unitOfWork.SaveChangesAsync();
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("res not true");
                        return result;
                    }
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("admin Value not true");
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

        //public async Task<ApiResponse<bool>> EditAdminById(editAdministrator adminDto)
        //{
        //    ApiResponse<bool> result = new ApiResponse<bool>();
        //    try
        //    {
        //        Administrator adminValue = await unitOfWork.AdministratorsManager.GetByIdAsync(adminDto.id);
        //        if (adminValue != null)
        //        {
        //            var userdata = await unitOfWork.UserManager.FindByIdAsync(adminValue.UserId);
        //            userdata.FirstName = adminDto.FirstName;
        //            userdata.LastName = adminDto.LastName;
        //            userdata.Email = adminDto.Email;
        //            userdata.PhoneNumber = adminDto.PhoneNumber;
        //            if (adminDto.password == null)
        //            {
        //                adminDto.password = userdata.PasswordHash;
        //            }
        //            else if (userdata.PasswordHash != adminDto.password)
        //            {
        //                var checkPassword = await unitOfWork.UserManager.CheckPasswordAsync(userdata, adminDto.password);
        //                if (checkPassword)
        //                {
        //                    var changePassword = await unitOfWork.UserManager.ChangePasswordAsync(userdata, adminDto.password, adminDto.newpassword);
        //                    if (changePassword != null)
        //                    {
        //                        var hasher = new PasswordHasher<ApplicationUser>();
        //                        userdata.PasswordHash = hasher.HashPassword(userdata, adminDto.newpassword);
        //                    }
        //                    else
        //                    {
        //                        result.Succeeded = false;
        //                        result.Errors.Add("change not true");
        //                        return result;
        //                    }
        //                }
        //                else
        //                {
        //                    result.Succeeded = false;
        //                    result.Errors.Add("check password not true");
        //                    return result;
        //                }
        //            }
        //            else
        //            {
        //                result.Succeeded = false;
        //                result.Errors.Add(" not true");
        //                return result;
        //            }
        //            var res = await unitOfWork.UserManager.UpdateAsync(userdata);
        //            if (res.Succeeded)
        //            {
        //                await unitOfWork.SaveChangesAsync();
        //                result.Data = true;
        //                result.Succeeded = true;
        //                return result;
        //            }
        //            else
        //            {
        //                result.Succeeded = false;
        //                result.Errors.Add("res not true");
        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            result.Succeeded = false;
        //            result.Errors.Add("admin Value not true");
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Succeeded = false;
        //        result.Errors.Add(ex.Message);
        //        return result;
        //    }
        //}

    }
 }


