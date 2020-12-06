using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class ModifyService
    {
        public int ServiceId { get; set; }
        public int oldPosition { get; set; }
        public int newPosition { get; set; }
    }
}
