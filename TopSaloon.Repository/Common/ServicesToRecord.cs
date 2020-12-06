using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.Repository.Common
{
    public class ServicesToRecord
    {
        public string ServiceNameAR { get; set; }
        public string ServiceNameEN { get; set; }
        public int? ServiceTime { get; set; }
        public float? ServicePrice { get; set; }
        //added fields 
    
        public bool? ServiceStatus { get; set; }
       
        

    }
}
