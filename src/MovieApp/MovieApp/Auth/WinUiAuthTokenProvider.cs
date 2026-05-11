namespace MovieApp.Auth
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using MovieApp.Proxy;

    /// <summary>
    /// Obtains a JWT token from the WebApi at startup and provides it to proxy services.
    /// </summary>
    public sealed class WinUiAuthTokenProvider : IAuthTokenProvider
    {
        private const string LoginEndpoint = "api/auth/login";
        private const string DefaultUsername = "admin";
        private const string DefaultPassword = "password123";
        private const string BaseUrl = "http://localhost:4544/";

        private string? token;

        /// <summary>
        /// Logs in to the WebApi and stores the JWT token.
        /// Call once before building the DI container.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                using HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    LoginEndpoint,
                    new { Username = DefaultUsername, Password = DefaultPassword });

                if (response.IsSuccessStatusCode)
                {
                    LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    this.token = result?.Token;
                }
            }
            catch
            {
                // App can still start without a token; all API calls will return 401.
            }
        }

        /// <inheritdoc/>
        public string? GetToken() => this.token;

        private sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
    }
}
