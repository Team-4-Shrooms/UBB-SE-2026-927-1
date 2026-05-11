using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

internal sealed class ProxyRepoIntegrationTestContext : IDisposable
{
    public ProxyRepoIntegrationTestContext()
    {
        Factory = new MovieAppWebApplicationFactory();
        HttpClient = Factory.CreateClient();
        ApiClient = new ApiClient(HttpClient, new NullAuthTokenProvider());
    }

    public MovieAppWebApplicationFactory Factory { get; }

    public HttpClient HttpClient { get; }

    public ApiClient ApiClient { get; }

    public void Dispose()
    {
        HttpClient.Dispose();
        Factory.Dispose();
    }
}
