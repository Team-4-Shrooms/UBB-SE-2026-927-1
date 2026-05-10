using System;

namespace MovieApp.Proxy
{
    public interface IAuthTokenProvider
    {
        string? GetToken();
    }
}
