using System;
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
    public class BarberLoginsManager : Repository<BarberLogin, ApplicationDbContext>
    {
        public BarberLoginsManager(ApplicationDbContext _context) : base(_context)
        {

        }

        public async Task<int> GetSignedInbarbers(DateRangeDTO dateRange)
         {

            return await Task.Run(() =>
            {
                

                int Result = 0;

                Result = context.BarberLogins.Where(A => A.LoginDateTime.Value.Date <= dateRange.EndDate.Date && A.LoginDateTime.Value.Date >= dateRange.StartDate.Date).Count();
               

                return Result;

            });
        }
        public async Task<int> GetSignedInbarbers()
        {

            return await Task.Run(() =>
            {
                DateTime myday = DateTime.Now;

                int Result = context.BarberLogins.Where(A => A.LoginDateTime.Value.Date == myday.Date).Distinct().Count();
                Result = Result + 0; 

                return Result;

            });
        }

    }
}
