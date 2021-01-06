using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class SendSMSDTO
    {
        //public string SMSText { get; set; }
       // public string SMSLink { get; set; }
        public string CustomerNumber { get; set; }
        public int orderId { get; set; }
    }
}
