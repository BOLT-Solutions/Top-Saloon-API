using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class EditBarberDTO
    {
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public int  Id { get; set; }
        public string status { get; set; }
        public string BarberProfilePhotoPathAdmin { get; set; }
        public string BarberProfilePhotoPathUser { get; set; }
        public string BarberFingerprintId { get; set; }
    }
}
