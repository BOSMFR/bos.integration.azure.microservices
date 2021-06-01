namespace BOS.Integration.Azure.Microservices.Domain.DTOs
{
    public class DownloadFileDTO
    {
        public byte[] Content { get; set; }

        public string ContentType { get; set; }
    }
}
