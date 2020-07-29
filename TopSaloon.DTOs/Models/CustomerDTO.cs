﻿using System;
using System.Collections.Generic;
using System.Text;
using TopSalon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UniqueCode { get; set; }
        public int? LastBarberId { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int? TotalNumberOfVisits { get; set; }
        public virtual List<OrderDTO> Orders { get; set; }
    }
}