using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? LastBarberId { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int? TotalNumberOfVisits { get; set; }
        public List<CompleteOrderDTO> CompleteOrders { get; set; }
        public bool isDeleted { get; set; }

    }
}
