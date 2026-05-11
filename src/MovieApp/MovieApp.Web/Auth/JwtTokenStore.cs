namespace MovieApp.Web.Auth
{
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Proxy;

    /// <summary>
    /// Holds the JWT token obtained at startup.
    /// Registered as a singleton and implements both
    /// <see cref="IAuthTokenProvider"/> (used by proxy services to attach the Bearer header)
    /// and <see cref="ICurrentUserService"/> (used by controllers to get the current user ID).
    /// </summary>
    public sealed class JwtTokenStore : IAuthTokenProvider, ICurrentUserService
    {
        private string? token;
        private int userId;

        /// <inheritdoc/>
        public string? GetToken() => this.token;

        /// <inheritdoc/>
        public int UserId => this.userId;

        /// <summary>
        /// Stores the token and user ID after a successful login.
        /// </summary>
        /// <param name="token">The JWT token string.</param>
        /// <param name="userId">The authenticated user's ID.</param>
        public void SetToken(string token, int userId)
        {
            this.token = token;
            this.userId = userId;
        }
    }
}
