namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Trailer Scraping feature.</summary>
    public sealed class TrailerScrapingController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Trailer Scraping";
            return this.View();
        }
    }
}
