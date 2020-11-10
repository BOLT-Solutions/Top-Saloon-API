
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.Core;
using TopSaloon.Core.Managers;
using TopSaloon.DTOs;
using TopSaloon.DTOs.Enums;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.Repository.Common;
using TopSaloon.ServiceLayer;

namespace TopSaloon.ServiceLayer
{
    public class PromoCodeService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public PromoCodeService(UnitOfWork unitOfWork,IConfiguration config, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<PromoCodeDTO>> CreatePromoCode(CreatePromoCodeDTO model)
        {
            ApiResponse<PromoCodeDTO> result = new ApiResponse<PromoCodeDTO>();
            try
            {
                PromoCode promoCodeToCreate = new PromoCode();
                promoCodeToCreate.CreationDate = DateTime.Now;
                promoCodeToCreate.ExpiryDate = model.ExpiryDate;
                promoCodeToCreate.UsageCount = model.UsageCount;
                promoCodeToCreate.DiscountRate = model.DiscountRate;

                if(model.DiscountRate > 100)
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error creating promo code , discount rate is more than 100% !");
                    return result;
                }

                string code;

                while(true)
                {
                    code = HelperFunctions.GenerateRandomNumber();

                    var promoCodeResult = await unitOfWork.PromoCodeManager.GetAsync(a => a.Code == code);

                    List<PromoCode> promoCodesList = promoCodeResult.ToList();

                    if(promoCodesList.Count == 0)
                    {
                        break;
                    }

                }


                promoCodeToCreate.Code = code;

                var createPromoCodeResult = await unitOfWork.PromoCodeManager.CreateAsync(promoCodeToCreate);

                await unitOfWork.SaveChangesAsync();

                if(createPromoCodeResult != null)
                {
                    result.Data = mapper.Map<PromoCodeDTO>(createPromoCodeResult);
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Error creating promo code");
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

        public async Task<ApiResponse<bool>> DeletePromoCode(DeletePromoCodeDTO model)
        {
            ApiResponse<bool> result = new ApiResponse<bool>();
            try
            { 
                var promoCodeResult = await unitOfWork.PromoCodeManager.GetByIdAsync(model.Id);

                if (promoCodeResult != null)
                {
                    var deletePromoCodeResult = await unitOfWork.PromoCodeManager.RemoveAsync(promoCodeResult);

                    await unitOfWork.SaveChangesAsync();

                    if(deletePromoCodeResult == true)
                    {
                        result.Data = true;
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to delete promo code !");
                        return result;
                    }      
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find promo a promo code with the specified id !");
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

        public async Task<ApiResponse<PromoCodeDTO>> EditPromoCode(PromoCodeDTO model)
        {
            ApiResponse<PromoCodeDTO> result = new ApiResponse<PromoCodeDTO>();
            try
            {
                var promoCodeResult = await unitOfWork.PromoCodeManager.GetByIdAsync(model.Id);

                if (promoCodeResult != null)
                {

                    promoCodeResult.DiscountRate = model.DiscountRate;
                    promoCodeResult.UsageCount = model.UsageCount;
                    promoCodeResult.ExpiryDate = model.ExpiryDate;

                    var updatePromoCodeResult = await unitOfWork.PromoCodeManager.UpdateAsync(promoCodeResult);

                    await unitOfWork.SaveChangesAsync();

                    if(updatePromoCodeResult == true)
                    {
                        result.Data = mapper.Map<PromoCodeDTO>(promoCodeResult);
                        result.Succeeded = true;
                        return result;
                    }
                    else
                    {
                        result.Succeeded = false;
                        result.Errors.Add("Failed to update promo code !");
                        return result;
                    }

                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to find promo a promo code with the specified id !");
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

        public async Task<ApiResponse<List<PromoCodeDTO>>> GetAllPromoCodes()
        {
            ApiResponse<List<PromoCodeDTO>> result = new ApiResponse<List<PromoCodeDTO>>();
            try
            {
                var getPromoCodeListResult = await unitOfWork.PromoCodeManager.GetAsync();

                if (getPromoCodeListResult != null)
                {
                    result.Data = mapper.Map<List<PromoCodeDTO>>(getPromoCodeListResult.ToList());
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    result.Succeeded = false;
                    result.Errors.Add("Failed to fetch promo codes !");
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


