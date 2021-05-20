using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder
{
    public class PickOrderFilterDTO
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
