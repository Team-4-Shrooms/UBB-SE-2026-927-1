using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing movie events.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Retrieves a list of movie events associated with a specific movie ID. 
        /// Each event represents a screening or showing of the movie,
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie for which to retrieve events. 
        /// Must correspond to an existing movie.</param>
        /// <returns>List of <see cref="MovieEvent"/> objects representing the events for the specified movie.
        List<MovieEvent> GetEventsForMovie(int movieId);

        /// <summary>
        /// Retrieves a list of all movie events available in the data source. 
        /// Each event represents a screening or showing of a movie,
        /// </summary>
        /// <returns>List of <see cref="MovieEvent"/> objects representing all available events. 
        /// The list will be empty if there are no events</returns>
        List<MovieEvent> GetAllEvents();

        /// <summary>
        /// Retrieves a movie event with the specified identifier.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event to retrieve. Must be a non-negative value and correspond to an 
        /// existing event.</param>
        /// <returns>A <see cref="MovieEvent"/> instance that matches the specified <paramref name="eventId"/>, 
        /// or <see langword="null"/> if no such event exists.</returns>
        MovieEvent? GetEventById(int eventId);

        /// <summary>
        /// Purchases a ticket for a specified user and event.
        /// </summary>
        /// <param name="userId">The unique identifier of the user making the purchase. Must correspond to an existing user.</param>
        /// <param name="eventId">The unique identifier of the event for which the ticket is being purchased. Must correspond to an available event.</param>
        void PurchaseTicket(int userId, int eventId);
    }
}
