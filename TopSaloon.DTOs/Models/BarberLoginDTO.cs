using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BarberLoginDTO
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DateTime? LoginDateTime { get; set; }
        public DateTime? logoutDateTime { get; set; }

    }
}
