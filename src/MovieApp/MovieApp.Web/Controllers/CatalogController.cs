namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Movie Catalog feature.</summary>
    public sealed class CatalogController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Movie Catalog";
            return this.View();
        }
    }
}
