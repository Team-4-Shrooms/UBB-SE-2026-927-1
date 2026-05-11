namespace MovieApp.Auth
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
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
        private static readonly TimeSpan LoginTimeout = TimeSpan.FromSeconds(5);

        private string? token;

        /// <summary>
        /// Logs in to the WebApi and stores the JWT token.
        /// Must be called via Task.Run to avoid deadlocking the UI thread.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                using HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
                using CancellationTokenSource cts = new CancellationTokenSource(LoginTimeout);

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    LoginEndpoint,
                    new { Username = DefaultUsername, Password = DefaultPassword },
                    cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>(
                        cancellationToken: cts.Token);
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
