using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Event WebAPI endpoints.
    /// </summary>
    public sealed class EventEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public EventEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllEvents_SeededDatabase_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/events");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllEvents_SeededDatabase_ReturnsNonEmptyList()
        {
            List<MovieEventDto>? allEvents = await _httpClient.GetFromJsonAsync<List<MovieEventDto>>("/api/events");

            Assert.NotEmpty(allEvents!);
        }

        [Fact]
        public async Task GetAllEvents_SeededDatabase_FirstEventHasNonEmptyTitle()
        {
            List<MovieEventDto>? allEvents = await _httpClient.GetFromJsonAsync<List<MovieEventDto>>("/api/events");

            Assert.False(string.IsNullOrWhiteSpace(allEvents![0].Title));
        }

        [Fact]
        public async Task GetAllEvents_SeededDatabase_AllEventsHaveMovieReference()
        {
            List<MovieEventDto>? allEvents = await _httpClient.GetFromJsonAsync<List<MovieEventDto>>("/api/events");

            bool allHaveMovieReference = allEvents!.All(movieEvent => movieEvent.Movie is not null);

            Assert.True(allHaveMovieReference);
        }

        [Fact]
        public async Task GetEventById_EventExists_ReturnsOkStatusCode()
        {
            List<MovieEventDto>? allEvents = await _httpClient.GetFromJsonAsync<List<MovieEventDto>>("/api/events");
            int existingEventId = allEvents![0].Id;

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/events/{existingEventId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetEventById_EventExists_ReturnsEventWithMatchingId()
        {
            List<MovieEventDto>? allEvents = await _httpClient.GetFromJsonAsync<List<MovieEventDto>>("/api/events");
            int existingEventId = allEvents![0].Id;

            MovieEventDto? foundEvent = await _httpClient.GetFromJsonAsync<MovieEventDto>($"/api/events/{existingEventId}");

            Assert.Equal(existingEventId, foundEvent!.Id);
        }

        [Fact]
        public async Task GetEventsForMovie_SeededDatabase_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/events/movie/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
