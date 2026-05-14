namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICurrentUserService _currentUser;

        public InventoryController(IInventoryService inventoryService, ICurrentUserService currentUser)
        {
            _inventoryService = inventoryService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Inventory";

            var userId = _currentUser.UserId;
            var moviesTask = _inventoryService.GetOwnedMoviesAsync(userId);
            var ticketsTask = _inventoryService.GetOwnedTicketsAsync(userId);
            var equipmentTask = _inventoryService.GetOwnedEquipmentAsync(userId);
            await Task.WhenAll(moviesTask, ticketsTask, equipmentTask);

            ViewBag.Movies = moviesTask.Result;
            ViewBag.Tickets = ticketsTask.Result;
            ViewBag.Equipment = equipmentTask.Result;

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMovie(int movieId)
        {
            try
            {
                await _inventoryService.RemoveOwnedMovieAsync(_currentUser.UserId, movieId);
                TempData["Success"] = "Movie removed from your inventory.";
            }
            catch
            {
                TempData["Error"] = "Failed to remove movie.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTicket(int eventId)
        {
            try
            {
                await _inventoryService.RemoveOwnedTicketAsync(_currentUser.UserId, eventId);
                TempData["Success"] = "Ticket removed from your inventory.";
            }
            catch
            {
                TempData["Error"] = "Failed to remove ticket.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEquipment(int equipmentId)
        {
            try
            {
                await _inventoryService.RemoveOwnedEquipmentAsync(_currentUser.UserId, equipmentId);
                TempData["Success"] = "Equipment removed from your inventory.";
            }
            catch
            {
                TempData["Error"] = "Failed to remove equipment.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
