using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieApp.Proxy
{
    public class ApiClient
    {
        private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly HttpClient _httpClient;
        private readonly IAuthTokenProvider _tokenProvider;

        public ApiClient(HttpClient httpClient, IAuthTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            
            // Note: The base address should be configured during DI setup (e.g., in App.xaml.cs).
        }

        private void AttachToken()
        {
            var token = _tokenProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> GetAsync<T>(string uri)
        {
            AttachToken();
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(DeserializeOptions);
        }

        public async Task PostAsync<TValue>(string uri, TValue value)
        {
            AttachToken();
            var response = await _httpClient.PostAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TResponse?> PostAsync<TValue, TResponse>(string uri, TValue value)
        {
            AttachToken();
            var response = await _httpClient.PostAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions);
        }

        public async Task PutAsync<TValue>(string uri, TValue value)
        {
            AttachToken();
            var response = await _httpClient.PutAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TResponse?> PutAsync<TValue, TResponse>(string uri, TValue value)
        {
            AttachToken();
            var response = await _httpClient.PutAsJsonAsync(uri, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions);
        }

        public async Task DeleteAsync(string uri)
        {
            AttachToken();
            var response = await _httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}
