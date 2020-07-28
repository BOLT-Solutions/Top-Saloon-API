﻿using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;

namespace TopSaloon.Core.Managers
{
    public class BarbersManager : Repository<Barber, ApplicationDbContext>
    {
        public BarbersManager(ApplicationDbContext _context) : base(_context)
        {

        }



    }
}