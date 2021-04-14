using System;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PackshotFilterDTO
    {
        public string StyleNo { get; set; }

        public string StyleName { get; set; }

        public string ColorNo { get; set; }

        public string ColorName { get; set; }

        public string Collection { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
