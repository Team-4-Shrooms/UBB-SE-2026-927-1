using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Repositories
{
    public interface IEventRepository
    {
        List<MovieEvent> GetEventsForMovie(int movieId);

        List<MovieEvent> GetAllEvents();

        MovieEvent? GetEventById(int eventId);

        void PurchaseTicket(int userId, int eventId);
    }
}
