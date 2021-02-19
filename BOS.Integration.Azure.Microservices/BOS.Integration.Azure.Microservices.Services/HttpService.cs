using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class HttpService : IHttpService
    {
        private readonly ILogger<HttpService> logger;

        public HttpService(ILogger<HttpService> logger)
        {
            this.logger = logger;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            using (var client = new HttpClient())
            {
                using (var httpResponse = await client.GetAsync(url))
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var content = await httpResponse.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    else
                    {
                        this.logger.LogError($"Failed to get by the URL: {url}");
                        return default;
                    }
                }
            }
        }

        public async Task<V> PostAsync<T, V>(string url, T dataParams, string key = null, string token = null)
        {
            using (var client = new HttpClient())
            {
                if (string.IsNullOrEmpty(key))
                {
                    return default;
                }

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var bodyContent = JsonConvert.SerializeObject(dataParams);
                var result = await client.PostAsync(url, new StringContent(bodyContent, Encoding.UTF8, "application/json"));  

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<V>(content);
                }
                else
                {
                    string errorMessage = $"Failed to post by the URL: {url}" + Environment.NewLine + $"Body: {bodyContent}";
                    this.logger.LogError(errorMessage);

                    return default;
                }
            }
        }
    }
}
