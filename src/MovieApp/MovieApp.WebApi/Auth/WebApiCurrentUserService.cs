namespace MovieApp.WebApi.Auth
{
    using System.IdentityModel.Tokens.Jwt;
    using MovieApp.Logic.Interfaces.Services;

    /// <summary>
    /// Reads the current user's ID from the JWT <c>sub</c> claim in the HTTP context.
    /// </summary>
    public sealed class WebApiCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiCurrentUserService"/> class.
        /// </summary>
        public WebApiCurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public int UserId
        {
            get
            {
                string? sub = this.httpContextAccessor.HttpContext?.User
                    .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                return int.TryParse(sub, out int id) ? id : 0;
            }
        }
    }
}
