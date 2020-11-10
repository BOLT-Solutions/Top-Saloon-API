
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
using TopSaloon.ServiceLayer;

namespace TopSaloon.ServiceLayer
{
    public class PromoCodeService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public PromoCodeService(UnitOfWork unitOfWork,IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

    }
}


