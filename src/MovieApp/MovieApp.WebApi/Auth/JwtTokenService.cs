namespace MovieApp.WebApi.Auth
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Generates signed JWT tokens for authenticated users.
    /// </summary>
    public sealed class JwtTokenService
    {
        private const int TokenLifetimeHours = 12;
        private readonly string secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="configuration">App configuration containing <c>Jwt:SecretKey</c>.</param>
        public JwtTokenService(IConfiguration configuration)
        {
            this.secretKey = configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        }

        /// <summary>
        /// Creates a signed HS256 JWT for the given user.
        /// </summary>
        /// <param name="userId">The user's database ID.</param>
        /// <param name="username">The user's username.</param>
        /// <returns>A tuple containing the token string and its expiry time.</returns>
        public (string Token, DateTime ExpiresAt) GenerateToken(int userId, string username)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.secretKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiresAt = DateTime.UtcNow.AddHours(TokenLifetimeHours);

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ];

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
