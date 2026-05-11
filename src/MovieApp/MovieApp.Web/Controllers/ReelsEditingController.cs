namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Reels Editing feature.</summary>
    public sealed class ReelsEditingController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Reels Editing";
            return this.View();
        }
    }
}
