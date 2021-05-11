using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix.Limit
{
    public class LimitErrorDTO
    {
        public List<LimitInnerErrorDTO> Errors { get; set; }

        public string Msg { get; set; }
    }
}
