using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieApp.Logic.Http
{
    public class ApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

        private readonly HttpClient _httpClient;
        
        // Hardcoded for now, but can inject a session service later
        private readonly int _userId = 1; 

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<T>> GetAllAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(AppendUserIdQuery(endpoint));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions) ?? new List<T>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Cannot connect to the server. Please ensure the Web API is running.", ex);
            }
        }

        public async Task<T?> GetByIdAsync<T>(string endpoint, int id)
        {
            try
            {
                var response = await _httpClient.GetAsync(AppendUserIdQuery($"{endpoint}/{id}"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Cannot connect to the server.", ex);
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(AppendUserIdQuery(endpoint), data, JsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to save data. Please check your connection.", ex);
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, int id, T data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(AppendUserIdQuery($"{endpoint}/{id}"), data, JsonOptions);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to update data. Please check your connection.", ex);
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(AppendUserIdQuery($"{endpoint}/{id}"));
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete data. Please check your connection.", ex);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(AppendUserIdQuery(endpoint));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Cannot connect to the server. Please ensure the Web API is running.", ex);
            }
        }

        public async Task PostAsync(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(AppendUserIdQuery(endpoint), data, JsonOptions);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to save data. Please check your connection.", ex);
            }
        }

        public async Task<TResponse?> PostAsync<TResponse>(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(AppendUserIdQuery(endpoint), data, JsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to save data. Please check your connection.", ex);
            }
        }

        public async Task PutAsync(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(AppendUserIdQuery(endpoint), data, JsonOptions);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to update data. Please check your connection.", ex);
            }
        }

        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(AppendUserIdQuery(endpoint));
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete data. Please check your connection.", ex);
            }
        }

        private string AppendUserIdQuery(string endpoint)
        {
            return endpoint.Contains('?')
                ? $"http://localhost:4544/{endpoint}&userId={_userId}"
                : $"http://localhost:4544/{endpoint}?userId={_userId}";
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
