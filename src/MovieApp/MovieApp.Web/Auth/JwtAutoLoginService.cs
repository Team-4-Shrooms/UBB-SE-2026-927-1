namespace MovieApp.Web.Auth
{
    using System.Net.Http.Json;

    /// <summary>
    /// Background service that logs in to the WebApi at startup and stores the JWT token.
    /// If login fails the app continues running without a token (all API calls will get 401).
    /// </summary>
    public sealed class JwtAutoLoginService : IHostedService
    {
        private readonly JwtTokenStore tokenStore;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<JwtAutoLoginService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAutoLoginService"/> class.
        /// </summary>
        public JwtAutoLoginService(
            JwtTokenStore tokenStore,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<JwtAutoLoginService> logger)
        {
            this.tokenStore = tokenStore;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string? username = this.configuration["Auth:Username"];
            string? password = this.configuration["Auth:Password"];
            string? baseUrl = this.configuration["WebApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(baseUrl))
            {
                this.logger.LogWarning("Auth credentials or WebApi base URL not configured — skipping auto-login.");
                return;
            }

            try
            {
                HttpClient client = this.httpClientFactory.CreateClient();
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    $"{baseUrl.TrimEnd('/')}/api/auth/login",
                    new { Username = username, Password = password },
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError("Auto-login failed with status {Status}.", response.StatusCode);
                    return;
                }

                LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
                if (result is null)
                {
                    this.logger.LogError("Auto-login returned an empty response.");
                    return;
                }

                this.tokenStore.SetToken(result.Token, result.UserId);
                this.logger.LogInformation("Auto-login succeeded for user {UserId}.", result.UserId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Auto-login threw an exception — app will start without a token.");
            }
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
    }
}
