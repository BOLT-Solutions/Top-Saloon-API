using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //public async Task<List<Barber>> getallBarbers()
        //{
        //    return await Task.Run(() =>
        //    {
        //        List<Barber> serviceFeedBackQuestions = context.Barbers.ToList();
        //        return serviceFeedBackQuestions;
        //    });
        //}
        public async Task<List<Barber>> GetAllAvailableBarber()
        {
            return await Task.Run(() =>
            {
                  List<Barber> barbers = context.Barbers.Where(a=> a.Status== "Available" && a.isDeleted==false).ToList();
                   return barbers;
                
            });
        }
        public async Task<int> GetNumberOfAvailableBarber()
        {
            return await Task.Run(() =>
            {
                int CountOfBarber = context.Barbers.Where(a => a.Status == "Available" && a.isDeleted == false).ToList().Count();
                return CountOfBarber;

            });
        }
    }
}
