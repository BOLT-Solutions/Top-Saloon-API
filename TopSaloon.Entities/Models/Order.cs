﻿using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Order
    {
        public int Id { get; set; }
        public float? OrderTotal { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? WaitingTimeInMinutes { get; set; }
        public int? OrderIdentifier { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
        public int BarberQueueId { get; set; }
        public int CustomerId { get; set; }
        public virtual BarberQueue BarberQueue { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List<OrderFeedback> OrderFeedbacks { get; set; }
        public virtual List<OrderService> OrderServices { get; set; }
    }
}