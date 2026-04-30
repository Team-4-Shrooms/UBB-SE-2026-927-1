using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MovieApp.Http
{
    public class ApiClient
    {
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
                var response = await _httpClient.GetAsync($"{endpoint}?userId={_userId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
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
                var response = await _httpClient.GetAsync($"{endpoint}/{id}?userId={_userId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
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
                var response = await _httpClient.PostAsJsonAsync($"{endpoint}?userId={_userId}", data);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
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
                var response = await _httpClient.PutAsJsonAsync($"{endpoint}/{id}?userId={_userId}", data);
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
                var response = await _httpClient.DeleteAsync($"{endpoint}/{id}?userId={_userId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete data. Please check your connection.", ex);
            }
        }
    }
}