using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string CustomerMobile { get; set; }

        public float? OrderTotal { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
        public int? TotalServicesWaitingTime { get; set; }
        public int? OrderIdentifier { get; set; }
        public float? DiscountRate { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public int BarberQueueId { get; set; }
        public virtual BarberQueueDTO BarberQueue { get; set; }
        public virtual List<OrderServiceDTO> OrderServices { get; set; }
    }
}
