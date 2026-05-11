namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Reels Feed feature.</summary>
    public sealed class ReelsFeedController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Reels Feed";
            return this.View();
        }
    }
}
