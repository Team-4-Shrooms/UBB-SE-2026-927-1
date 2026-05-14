namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Web.Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class MarketplaceController : Controller
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly IEquipmentService _equipmentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _environment;

        public MarketplaceController(IEquipmentService equipmentService, ICurrentUserService currentUserService, IWebHostEnvironment environment)
        {
            _equipmentService = equipmentService;
            _currentUserService = currentUserService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var equipment = await _equipmentService.GetAvailableEquipmentAsync();
            if (!string.IsNullOrEmpty(search))
            {
                equipment = equipment.Where(e => e.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(equipment);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            ViewBag.CurrentUserId = _currentUserService.UserId;
            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            try
            {
                await _equipmentService.PurchaseEquipmentAsync(id, _currentUserService.UserId, equipment.Price, "User Address Placeholder");
                return RedirectToAction("Index", "Inventory");
            }
            catch (Exception ex)
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

            // URL takes priority over uploaded file — matches Windows logic
            string finalImageUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(form.ImageUrl))
            {
                finalImageUrl = form.ImageUrl;
            }
            else if (form.ImageFile != null && form.ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(form.ImageFile.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(ext))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, .png, and .webp files are allowed.");
                    return View(form);
                }

                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var uniqueName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, uniqueName);
                await using var stream = System.IO.File.Create(filePath);
                await form.ImageFile.CopyToAsync(stream);
                finalImageUrl = $"/uploads/{uniqueName}";
            }

            var equipment = new Equipment
            {
                Title = form.Name,
                Description = form.Description,
                Category = form.Category,
                Condition = form.Condition,
                Price = form.Price,
                ImageUrl = finalImageUrl,
                Status = EquipmentStatus.Available,
                Seller = new User { Id = _currentUserService.UserId }
            };

            await _equipmentService.ListItemAsync(equipment);
            return RedirectToAction("Index");
        }
    }
}
