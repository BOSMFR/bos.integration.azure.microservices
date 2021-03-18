using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Collection
{
    public class CollectionLogsDTO
    {
        public CollectionEntity Collection { get; set; }

        public LogDTO Logs { get; set; }
    }
}
