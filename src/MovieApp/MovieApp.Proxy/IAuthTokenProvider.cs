using System.Threading.Tasks;

namespace MovieApp.Proxy
{
    public interface IAuthTokenProvider
    {
        string? GetToken();

        /// <summary>
        /// Attempts (or re-attempts) to acquire a fresh token.
        /// Called automatically by ApiClient when a request receives a 401 response.
        /// Implementations should be safe to call multiple times.
        /// </summary>
        Task RefreshAsync();
    }
}
