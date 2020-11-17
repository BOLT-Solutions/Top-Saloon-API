using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UniqueCode { get; set; }
        public int? LastBarberId { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int? TotalNumberOfVisits { get; set; }
        public List<CompleteOrder> CompleteOrders { get; set; }
        public bool isDeleted { get; set; }

    }
}
