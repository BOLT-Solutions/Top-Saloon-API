using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class AdministratorDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShopId { get; set; }
        public string Role { get; set; }
        public virtual ShopDTO Shop { get; set; }
    }
}
