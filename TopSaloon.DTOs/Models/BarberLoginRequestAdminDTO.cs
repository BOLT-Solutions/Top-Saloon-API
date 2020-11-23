using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BarberLoginRequestAdminDTO
    {
        public int BarberId { get; set; }
        public DateTime Time { get; set; }

        public BarberLoginRequestAdminDTO()
        {
            this.Time = this.Time.AddHours(2);
        }

    }
}
