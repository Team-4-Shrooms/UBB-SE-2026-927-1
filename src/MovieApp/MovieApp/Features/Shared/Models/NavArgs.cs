namespace MovieApp.Features.Shared.Models
{
    public sealed class MovieCatalogNavArgs
    {
        public bool ShowOnlySales { get; init; }
    }

    public sealed class MovieDetailNavArgs
    {
        public DataLayer.Models.Movie Movie { get; init; } = null!;
    }
}
