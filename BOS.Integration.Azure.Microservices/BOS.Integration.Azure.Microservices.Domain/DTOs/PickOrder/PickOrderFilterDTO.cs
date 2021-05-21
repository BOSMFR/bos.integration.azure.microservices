using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PickOrderFilterDTO
    {
        public string OrderNumber { get; set; }

        public string ReceiverName { get; set; }

        public string CustomerNumber { get; set; }

        public string CustomerId1 { get; set; }

        public string CustomerId2 { get; set; }

        public string CustomerId3 { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
