using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string url, string key = null, string token = null, PrimeCargoAuthRequestDTO authBody = null);

        Task<V> PostAsync<T, V>(string url, T dataParams, string key = null, string token = null) where V : HttpResponse, new();
    }
}
