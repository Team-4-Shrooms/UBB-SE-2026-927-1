namespace MovieApp.WebApi.Auth
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.DataLayer.Interfaces.Repositories;

    /// <summary>
    /// Handles authentication — issues JWT tokens on successful login.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JwtTokenService tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        public AuthController(IUserRepository userRepository, JwtTokenService tokenService)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>A JWT token with user ID and expiry, or 401 on failure.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return this.Unauthorized();
            }

            var user = await this.userRepository.GetUserByUsernameAsync(request.Username);
            if (user is null)
            {
                return this.Unauthorized();
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                return this.Unauthorized();
            }

            (string token, DateTime expiresAt) = this.tokenService.GenerateToken(user.Id, user.Username);

            return this.Ok(new LoginResponse(token, user.Id, expiresAt));
        }
    }

    /// <summary>Login request body.</summary>
    public sealed record LoginRequest(string Username, string Password);

    /// <summary>Successful login response.</summary>
    public sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
}
