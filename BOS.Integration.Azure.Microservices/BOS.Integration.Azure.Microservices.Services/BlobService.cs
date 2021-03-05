using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class BlobService : IBlobService
    {
        private readonly CloudBlobContainer blobContainer;

        public BlobService(IConfigurationManager configuration)
        {
            this.blobContainer = this.GetBlobContainer(configuration.AzureMainBlobContainer, configuration.AzureStorageConnectionString);
        }

        public async Task<string> DownloadFileByFileNameAsync(string fileName)
        {
            CloudBlockBlob blockBlob = this.blobContainer.GetBlockBlobReference(fileName);

            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private CloudBlobContainer GetBlobContainer(string containerName, string azureStorageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            if (!blobContainer.Exists())
            {
                blobContainer.Create();
            }

            return blobContainer;
        }
    }
}
