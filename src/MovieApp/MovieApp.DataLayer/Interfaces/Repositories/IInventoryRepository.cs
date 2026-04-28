using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines methods for accessing and managing a user's owned movies, tickets, and equipment in the inventory
    /// system.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieves the list of movies owned by the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose owned movies are to be retrieved. Must be a valid user ID.</param>
        /// <returns>A list of <see cref="Movie"/> objects representing the movies owned by the user. Returns an empty list if
        /// the user owns no movies.</returns>
        List<Movie> GetOwnedMovies(int userId);

        /// <summary>
        /// Removes the specified movie from the collection of movies owned by the given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose ownership record will be updated.</param>
        /// <param name="movieId">The unique identifier of the movie to remove from the user's owned collection.</param>
        void RemoveOwnedMovie(int userId, int movieId);

        /// <summary>
        /// Removes the specified ticket from the collection of tickets owned by the given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose ownership record will be updated.</param>
        /// <param name="eventId">The unique identifier of the event ticket to remove from the user's owned collection.</param>
        void RemoveOwnedTicket(int userId, int eventId);

        /// <summary>
        /// Retrieves the list of event tickets owned by the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose owned tickets are to be retrieved. Must be a valid user ID.</param>
        /// <returns>The list of <see cref="MovieEvent"/> objects representing the event tickets owned by the user. 
        /// Returns an empty list if the user owns no tickets.</returns>
        List<MovieEvent> GetOwnedTickets(int userId);

        /// <summary>
        /// Retrieves the list of equipment owned by the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose owned equipment is to be retrieved. Must be a valid user ID.</param>
        /// <returns>List of <see cref="Equipment"/> objects representing the equipment owned by the user. 
        /// Returns an empty list if the user owns none.</returns>
        List<Equipment> GetOwnedEquipment(int userId);
    }
}
