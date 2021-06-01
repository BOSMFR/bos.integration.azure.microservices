using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class BlobService : IBlobService
    {
        private const string FileNameMetadataKey = "FileName";
        private readonly CloudBlobContainer blobContainer;

        public BlobService(IConfigurationManager configuration)
        {
            this.blobContainer = this.GetBlobContainer(configuration.AzureMainBlobContainer, configuration.AzureStorageConnectionString);
        }

        public async Task<string> DownloadFileContentByFileNameAsync(string fileName)
        {
            CloudBlockBlob blockBlob = this.blobContainer.GetBlockBlobReference(fileName);

            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public async Task<ActionExecutionResult> DownloadFileByFileNameAsync(string fileName)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return null;
                }

                string contentType = (fileName.Split('.').Last()) switch
                {
                    "xml" => ContentTypes.Xml,
                    "jsom" => ContentTypes.Json,
                    _ => string.Empty,
                };

                CloudBlockBlob blockBlob = this.blobContainer.GetBlockBlobReference(fileName);

                byte[] content;

                using (var stream = new MemoryStream())
                {
                    stream.Position = 0;
                    await blockBlob.DownloadToStreamAsync(stream);
                    await stream.CopyToAsync(stream);

                    content = stream.ToArray();
                }

                actionResult.Entity = new DownloadFileDTO
                {
                    Content = content,
                    ContentType = contentType
                };

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task UploadFileToBlobContainer(string fileName, string base64File)
        {
            CloudBlockBlob blockBlob = this.blobContainer.GetBlockBlobReference(fileName);

            using (var ms = new MemoryStream())
            {
                var fileBytes = Convert.FromBase64String(base64File);
                await blockBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
            }

            blockBlob.Metadata[FileNameMetadataKey] = fileName;
            await blockBlob.SetMetadataAsync();

            await blockBlob.SetPropertiesAsync();
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
