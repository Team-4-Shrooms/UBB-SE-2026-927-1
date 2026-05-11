namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>Stub controller for the Events feature.</summary>
    public sealed class EventsController : Controller
    {
        /// <summary>Placeholder index page.</summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Events";
            return this.View();
        }
    }
}
