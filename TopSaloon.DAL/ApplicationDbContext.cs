using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.Entities.Models;

namespace TopSaloon.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Barber> Barbers { get; set; }
        public virtual DbSet<BarberProfilePhoto> BarberProfilePhotos { get; set; }
        public virtual DbSet<BarberQueue> BarberQueues { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DailyReport> DailyReports { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderFeedback> OrderFeedbacks { get; set; }
        public virtual DbSet<ServiceFeedBackQuestion> ServiceFeedBackQuestions { get; set; }
        public virtual DbSet<OrderFeedbackQuestion> OrderFeedbackQuestions { get; set; }
        public virtual DbSet<OrderService> OrderServices { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SMS> SMS { get; set; }
        public virtual DbSet<CompleteOrder> CompleteOrders { get; set; }
        public virtual DbSet<BarberLogin> BarberLogins { get; set; }
        public virtual DbSet<GuestNumber> GuestNumber { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }



    }
}
