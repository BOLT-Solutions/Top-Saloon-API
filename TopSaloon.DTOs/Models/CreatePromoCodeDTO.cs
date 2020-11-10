using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class CreatePromoCodeDTO
    {
       public DateTime ExpiryDate { get; set; }
       public int UsageCount { get; set; }
       public float DiscountRate { get; set; }

    }
}
