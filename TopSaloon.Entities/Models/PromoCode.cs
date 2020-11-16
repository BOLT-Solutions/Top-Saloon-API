using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class PromoCode
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
