namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.DataLayer.Models;
    using MovieApp.Web.Models;
    using System.Threading.Tasks;
    using System.Linq;

    public sealed class MarketplaceController : Controller
    {
        private readonly IEquipmentService _equipmentService;
        private readonly ICurrentUserService _currentUserService;

        public MarketplaceController(IEquipmentService equipmentService, ICurrentUserService currentUserService)
        {
            _equipmentService = equipmentService;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var equipment = await _equipmentService.GetAvailableEquipmentAsync();
            if (!string.IsNullOrEmpty(search))
            {
                equipment = equipment.Where(e => e.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return View(equipment);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null) return NotFound();

            ViewBag.CurrentUserId = _currentUserService.UserId;
            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null) return NotFound();

            try
            {
                await _equipmentService.PurchaseEquipmentAsync(id, _currentUserService.UserId, equipment.Price, "User Address Placeholder");
                return RedirectToAction("Index", "Inventory");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Detail", new { id });
            }
        }

        [HttpGet]
        public IActionResult Sell()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell(SellEquipmentForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var equipment = new Equipment
            {
                Title = form.Name,
                Description = form.Description,
                Category = form.Category,
                Condition = form.Condition,
                Price = form.Price,
                ImageUrl = form.ImageUrl,
                Status = EquipmentStatus.Available,
                Seller = new User { Id = _currentUserService.UserId }
            };

            await _equipmentService.ListItemAsync(equipment);
            return RedirectToAction("Index");
        }
    }
}
