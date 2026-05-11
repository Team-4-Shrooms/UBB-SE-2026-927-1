namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Movie Tournament feature.</summary>
    public sealed class TournamentController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Movie Tournament";
            return this.View();
        }
    }
}
