﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSaloon.DAL;
using TopSaloon.DTOs.Models;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class AdministratorsManager : Repository<Administrator, ApplicationDbContext>
    {
        public AdministratorsManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public AdministratorDTO GetAdminByUserId (string userId)
        {
            var admin = context.Administrators.Where(a => a.UserId == userId).FirstOrDefault();
            AdministratorDTO adminDTO = new AdministratorDTO();
            adminDTO.Id = admin.Id;
            adminDTO.UserId = admin.UserId;
            return adminDTO;
        }


    }
}
