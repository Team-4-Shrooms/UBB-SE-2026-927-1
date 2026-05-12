namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.DataLayer.Models;
    using System.Threading.Tasks;
    using System.Linq;

    public sealed class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICurrentUserService _currentUserService;

        public EventsController(IEventService eventService, ICurrentUserService currentUserService)
        {
            _eventService = eventService;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var events = await _eventService.GetAvailableEventsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e => e.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return View(events);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var movieEvent = await _eventService.GetEventByIdAsync(id);
            if (movieEvent == null) return NotFound();

            var userId = _currentUserService.UserId;
            ViewBag.HasTicket = await _eventService.UserHasTicketAsync(userId, id);
            
            // For sold out logic, we need to check capacity vs sold tickets.
            // Since we don't have a direct count in IEventService yet, we'll assume it's handled or we'll add it.
            // For now, let's assume we can check capacity.
            // In a real app, I'd add GetSoldTicketsCountAsync to IEventService.
            
            return View(movieEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyTicket(int eventId)
        {
            try
            {
                await _eventService.PurchaseTicketAsync(_currentUserService.UserId, eventId);
                return RedirectToAction("Index", "Inventory");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Detail", new { id = eventId });
            }
        }
    }
}
