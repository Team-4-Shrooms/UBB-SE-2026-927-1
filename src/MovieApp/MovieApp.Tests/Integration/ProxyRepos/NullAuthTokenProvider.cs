using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos
{
    /// <summary>
    /// Provides a null auth token — no Authorization header is sent.
    /// Works in integration tests because the test factory bypasses JWT auth.
    /// </summary>
    internal sealed class NullAuthTokenProvider : IAuthTokenProvider
    {
        public string? GetToken() => null;
    }
}
