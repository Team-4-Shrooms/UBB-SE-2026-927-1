using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Proxy;

public class ReelInteractionProxyService : IReelInteractionService
{
    private readonly ReelInteractionService _inner;
    private readonly ApiClient _apiClient; 

    public ReelInteractionProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;

        _inner = new ReelInteractionService(
            new InteractionProxyRepository(apiClient),
            new PreferenceProxyRepository(apiClient));
    }

    public async Task ToggleLikeAsync(int userId, int reelId)
    {
        await _inner.ToggleLikeAsync(userId, reelId);
    }

    public async Task<int> GetLikeCountAsync(int reelId)
    {
        return await _inner.GetLikeCountAsync(reelId);
    }

    public Task RecordViewAsync(int userId, int reelId, double watchDurationSec, double watchPercentage)
    {
        return ((IReelInteractionService)_inner).RecordViewAsync(userId, reelId, watchDurationSec, watchPercentage);
    }

    public Task<UserReelInteraction?> GetInteractionAsync(int userId, int reelId)
    {
        return ((IReelInteractionService)_inner).GetInteractionAsync(userId, reelId);
    }

}
