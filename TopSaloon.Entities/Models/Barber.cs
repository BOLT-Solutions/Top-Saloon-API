using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Barber
    {
        public int Id { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string Status { get; set; }
        public int? NumberOfCustomersHandled { get; set; }
        public int ShopId { get; set; }
        public string BarberFingerPrintId { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual BarberProfilePhoto BarberProfilePhoto { get; set; }
        public virtual BarberQueue BarberQueue { get; set; }
        public bool isDeleted { get; set; }


    }
}
