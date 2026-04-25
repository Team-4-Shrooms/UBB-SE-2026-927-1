using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IInventoryRepository
    {
        List<Movie> GetOwnedMovies(int userId);

        void RemoveOwnedMovie(int userId, int movieId);

        void RemoveOwnedTicket(int userId, int eventId);

        List<MovieEvent> GetOwnedTickets(int userId);

        List<Equipment> GetOwnedEquipment(int userId);
    }
}
