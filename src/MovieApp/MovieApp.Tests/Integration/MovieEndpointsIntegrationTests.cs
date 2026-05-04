using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Movie WebAPI endpoints.
    /// </summary>
    public sealed class MovieEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public MovieEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetMovieById_MovieExists_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/movies/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMovieById_MovieExists_ReturnsNonNullBody()
        {
            MovieDto? movieDto = await _httpClient.GetFromJsonAsync<MovieDto>("/api/movies/1");

            Assert.NotNull(movieDto);
        }

        [Fact]
        public async Task GetMovieById_MovieExists_ReturnsMovieWithCorrectId()
        {
            MovieDto? movieDto = await _httpClient.GetFromJsonAsync<MovieDto>("/api/movies/1");

            Assert.Equal(1, movieDto!.Id);
        }

        [Fact]
        public async Task GetMovieById_MovieExists_ReturnsMovieWithNonEmptyTitle()
        {
            MovieDto? movieDto = await _httpClient.GetFromJsonAsync<MovieDto>("/api/movies/1");

            Assert.False(string.IsNullOrWhiteSpace(movieDto!.Title));
        }

        [Fact]
        public async Task GetMovieById_MovieDoesNotExist_ReturnsNoContentStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/movies/99999");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task SearchMovies_PartialTitleMatch_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/movies/search?partialMovieName=Inception");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SearchMovies_PartialTitleMatch_ReturnsNonEmptyList()
        {
            List<MovieDto>? matchingMovies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies/search?partialMovieName=Inception");

            Assert.NotEmpty(matchingMovies!);
        }

        [Fact]
        public async Task SearchMovies_PartialTitleMatch_AllResultsContainSearchTerm()
        {
            List<MovieDto>? matchingMovies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies/search?partialMovieName=Inception");

            bool allContainSearchTerm = matchingMovies!.All(movieDto => movieDto.Title.Contains("Inception", StringComparison.OrdinalIgnoreCase));

            Assert.True(allContainSearchTerm);
        }

        [Fact]
        public async Task SearchMovies_NoMatchingTitle_ReturnsEmptyList()
        {
            List<MovieDto>? matchingMovies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies/search?partialMovieName=ZZZZNONEXISTENT");

            Assert.Empty(matchingMovies!);
        }

        [Fact]
        public async Task UserOwnsMovie_UserDoesNotOwnMovie_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/movies/1/owned/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UserOwnsMovie_UserDoesNotOwnMovie_ReturnsFalse()
        {
            bool ownsMovie = await _httpClient.GetFromJsonAsync<bool>("/api/movies/1/owned/1");

            Assert.False(ownsMovie);
        }
    }
}
