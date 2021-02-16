using BOS.Integration.Azure.Microservices.Domain;
using System.Collections.Generic;

namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public class CosmosDbSettings
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseName { get; set; }

        public List<ContainerInfo> Containers { get; set; }
    }
}
