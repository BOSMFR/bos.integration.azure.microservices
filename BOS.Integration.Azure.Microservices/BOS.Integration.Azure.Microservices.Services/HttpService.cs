using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
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

        public async Task<T> GetAsync<T>(string url, string key = null, string token = null, PrimeCargoAuthRequestDTO authBody = null)
        {
            using (var client = new HttpClient())
            {
                if (authBody != null)
                {
                    client.DefaultRequestHeaders.Add("ownerCode", authBody.OwnerCode);
                    client.DefaultRequestHeaders.Add("username", authBody.UserName);
                    client.DefaultRequestHeaders.Add("password", authBody.Password);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                }

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

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

        public async Task<HttpExecutionResult> PostSoapAsync(string url, string xmlBody, string soapAction, string userName = null, string password = null)
        {
            var result = new HttpExecutionResult();

            HttpMessageHandler handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var httpClient = new HttpClient(handler))
            {
                httpClient.BaseAddress = new Uri(url);

                var content = new StringContent(xmlBody, Encoding.UTF8, "text/xml");

                httpClient.DefaultRequestHeaders.Add("SOAPAction", soapAction);

                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{userName}:{password}")));
                }

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                    {
                        result.Succeeded = true;
                        result.Content = stream.ReadToEnd();

                        return result;
                    }
                }
                else
                {
                    result.Error = $"Failed to post by the URL: {url}" + Environment.NewLine + $"Body: {xmlBody}";
                    this.logger.LogError(result.Error);

                    return result;
                }
            }
        }

        public async Task<V> PostAsync<T, V>(string url, T dataParams, string key = null, string token = null)
            where V : HttpResponse, new()
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(key))
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                }

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
                    var response = JsonConvert.DeserializeObject<V>(content);
                    response.StatusCode = Convert.ToInt32(HttpStatusCode.OK).ToString();

                    return response;
                }
                else
                {
                    string errorMessage = $"Failed to post by the URL: {url}" + Environment.NewLine + $"Body: {bodyContent}";
                    this.logger.LogError(errorMessage);

                    return result.StatusCode == HttpStatusCode.RequestTimeout ? new V { StatusCode = Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString() } : default;
                }
            }
        }
    }
}
