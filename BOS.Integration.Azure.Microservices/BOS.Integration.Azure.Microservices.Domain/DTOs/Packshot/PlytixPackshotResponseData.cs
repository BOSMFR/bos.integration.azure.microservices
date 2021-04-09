using System;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot
{
    public class PlytixPackshotResponseData
    {
        public bool Assigned { get; set; }

        public List<PlytixPackshotCategoryDTO> Categories { get; set; }

        public string ContentType { get; set; }

        public DateTime Created { get; set; }

        public string Extension { get; set; }

        public DateTime FileModified { get; set; }

        public int FileSize { get; set; }

        public string FileType { get; set; }

        public string Filename { get; set; }

        public bool HasCustomThumb { get; set; }

        public string Id { get; set; }

        public DateTime Modified { get; set; }

        public int NCatalogs { get; set; }

        public int NProducts { get; set; }

        public bool Public { get; set; }

        public string Status { get; set; }

        public string Thumbnail { get; set; }

        public string Url { get; set; }
    }
}
