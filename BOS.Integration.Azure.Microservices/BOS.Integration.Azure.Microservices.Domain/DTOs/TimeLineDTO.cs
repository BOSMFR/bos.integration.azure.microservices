using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class TimeLineDTO
    {
        public DateTime DateTime { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string InfoFileName { get; set; }
    }
}
