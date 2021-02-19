using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string url);

        Task<V> PostAsync<T, V>(string url, T dataParams, string key = null, string token = null);
    }
}
