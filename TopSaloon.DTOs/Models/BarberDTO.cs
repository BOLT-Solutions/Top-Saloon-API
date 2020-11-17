using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BarberDTO
    {
        public int Id { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string Status { get; set; }
        public int? NumberOfCustomersHandled { get; set; }
        public int ShopId { get; set; }
        public bool isDeleted { get; set; }

        public string BarberFingerPrintId { get; set; }
        public virtual ShopDTO Shop { get; set; }
        public virtual BarberProfilePhotoDTO BarberProfilePhoto { get; set; }
        public virtual BarberQueueDTO BarberQueue { get; set; }
    }
}
