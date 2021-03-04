using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class LogDTO
    {
        public ICollection<TimeLine> TimeLines { get; set; }

        public ICollection<ErpMessage> ErpMessages { get; set; }
    }
}
