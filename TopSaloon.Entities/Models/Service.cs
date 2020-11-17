using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Service
    {
        public int Id { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public string AdminPath { get; set; }
        public string UserPath { get; set; }
        public bool isDeleted { get; set; }

        public virtual List<ServiceFeedBackQuestion> FeedBackQuestions { get; set; }
    }
}
