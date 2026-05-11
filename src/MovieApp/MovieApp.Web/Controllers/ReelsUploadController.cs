namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Reels Upload feature.</summary>
    public sealed class ReelsUploadController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Reels Upload";
            return this.View();
        }
    }
}
