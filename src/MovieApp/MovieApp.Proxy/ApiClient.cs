using System;
using System.Net;
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
        }

        private void AttachToken()
        {
            var token = _tokenProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// If a response comes back as 401, try to acquire/refresh the token once and retry.
        /// This handles the case where the WebApi wasn't running when the app started.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithRetryAsync(Func<Task<HttpResponseMessage>> send)
        {
            AttachToken();
            HttpResponseMessage response = await send();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenProvider.RefreshAsync();
                AttachToken();
                response = await send();
            }

            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<T?> GetAsync<T>(string uri)
        {
            var response = await SendWithRetryAsync(() => _httpClient.GetAsync(uri));
            return await response.Content.ReadFromJsonAsync<T>(DeserializeOptions);
        }

        public async Task PostAsync<TValue>(string uri, TValue value)
        {
            await SendWithRetryAsync(() => _httpClient.PostAsJsonAsync(uri, value));
        }

        public async Task<TResponse?> PostAsync<TValue, TResponse>(string uri, TValue value)
        {
            var response = await SendWithRetryAsync(() => _httpClient.PostAsJsonAsync(uri, value));
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions);
        }

        public async Task PutAsync<TValue>(string uri, TValue value)
        {
            await SendWithRetryAsync(() => _httpClient.PutAsJsonAsync(uri, value));
        }

        public async Task<TResponse?> PutAsync<TValue, TResponse>(string uri, TValue value)
        {
            var response = await SendWithRetryAsync(() => _httpClient.PutAsJsonAsync(uri, value));
            return await response.Content.ReadFromJsonAsync<TResponse>(DeserializeOptions);
        }

        public async Task DeleteAsync(string uri)
        {
            await SendWithRetryAsync(() => _httpClient.DeleteAsync(uri));
        }
    }
}
