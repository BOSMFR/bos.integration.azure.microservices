using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IBlobService
    {
        Task<string> DownloadFileByFileNameAsync(string fileName);
    }
}
