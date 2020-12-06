using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.Repository.Common
{
    public class OrderToRecord
    {
        public string CustomerNameAR { get; set; }
        public string CustomerNameEN { get; set; }
        public string BarberNameAR { get; set; }
        public string BarberNameEN { get; set; }
        public float? DiscountPrice { get; set; }
        public float? DiscountRate { get; set; }
        public float? OrderTotalAmount { get; set; }
        public DateTime? OrderStartTime { get; set; }
        public DateTime? OrderEndTime { get; set; }
        public List<ServicesToRecord> Services { get; set; }
    }
}
