using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class LogDTO
    {
        public List<TimeLine> TimeLines { get; set; }

        public List<ErpMessage> ErpMessages { get; set; }
    }
}
