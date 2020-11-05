using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;
using TopSaloon.Repository;
using System.Threading.Tasks;
using System.Linq;


namespace TopSaloon.Core.Managers
{
    public class DailyReportsManager : Repository<DailyReport, ApplicationDbContext>
    {
        public DailyReportsManager(ApplicationDbContext _context) : base(_context)
        {

        }
        public async Task<int> GetSigndInbarbers()
        {

            return await Task.Run(() =>
            {
                var myday = DateTime.Today;
        
                int Result = context.CompleteOrders.Where(A => A.OrderDateTime.Value.Date >= myday).Distinct().Count(); 

                return Result;

            });
        }
        public async Task<int> GetSigndInbarbers(DateTime lastdate)
        {

            return await Task.Run(() =>
            {
                var myday = DateTime.Today;

                int Result = context.CompleteOrders.Where(A => A.OrderDateTime >= lastdate && A.OrderDateTime <= myday).Distinct().Count();

                return Result;

            });
        }



    }
}
