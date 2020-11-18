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
        public List<ServicesToRecord> Services { get; set; }
    }
}
