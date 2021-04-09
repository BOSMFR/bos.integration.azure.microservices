namespace BOS.Integration.Azure.Microservices.Domain.Entities.Packshot
{
    public class File
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Created { get; set; }

        public string BlobContainerName { get; set; }

        public int SizeKb { get; set; }
    }
}
