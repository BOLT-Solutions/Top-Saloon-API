using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public partial class PromoCodeDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public float DiscountRate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int OriginalUsageCount { get; set; }
        public int UsageCount { get; set; }

    }
}
