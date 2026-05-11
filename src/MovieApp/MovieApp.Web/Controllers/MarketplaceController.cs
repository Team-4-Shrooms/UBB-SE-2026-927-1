namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Marketplace feature.</summary>
    public sealed class MarketplaceController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Marketplace";
            return this.View();
        }
    }
}
