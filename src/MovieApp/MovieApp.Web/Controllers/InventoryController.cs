namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Inventory feature.</summary>
    public sealed class InventoryController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Inventory";
            return this.View();
        }
    }
}
