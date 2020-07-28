﻿using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Service
    {
        public int Id { get; set; }
        public int? Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public virtual List<ServiceFeedBackQuestion> FeedBackQuestions { get; set; }
    }
}