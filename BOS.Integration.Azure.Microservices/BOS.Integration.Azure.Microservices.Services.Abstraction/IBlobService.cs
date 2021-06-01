using BOS.Integration.Azure.Microservices.Domain;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IBlobService
    {
        Task<string> DownloadFileContentByFileNameAsync(string fileName);

        Task<ActionExecutionResult> DownloadFileByFileNameAsync(string fileName);

        Task UploadFileToBlobContainer(string fileName, string base64File);
    }
}
