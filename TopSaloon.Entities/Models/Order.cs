using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Order
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
        public virtual BarberQueue BarberQueue { get; set; }
        public virtual List<OrderService> OrderServices { get; set; }
    }
}
