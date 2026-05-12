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

            int retryCount = 3;
            int delaySeconds = 2;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    HttpClient client = this.httpClientFactory.CreateClient();
                    HttpResponseMessage response = await client.PostAsJsonAsync(
                        $"{baseUrl.TrimEnd('/')}/api/auth/login",
                        new { Username = username, Password = password },
                        cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
                        if (result != null)
                        {
                            this.tokenStore.SetToken(result.Token, result.UserId);
                            this.logger.LogInformation("Auto-login succeeded for user {UserId} (attempt {Attempt}).", result.UserId, i + 1);
                            return;
                        }
                    }

                    this.logger.LogWarning("Auto-login attempt {Attempt} failed with status {Status}.", i + 1, response.StatusCode);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning("Auto-login attempt {Attempt} failed: {Message}", i + 1, ex.Message);
                }

                if (i < retryCount - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
                }
            }

            this.logger.LogError("Auto-login failed after {Count} attempts. App will start without a token.", retryCount);
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
    }
}
