using System;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class TimeLineFilterDTO
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public ICollection<string> Statuses { get; set; }

        public ICollection<string> Objects { get; set; }
    }
}
