namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class WalletController : Controller
    {
        private const int PageSize = 20;

        private readonly IProfileService _profileService;
        private readonly ICurrentUserService _currentUser;

        public WalletController(IProfileService profileService, ICurrentUserService currentUser)
        {
            _profileService = profileService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewData["Title"] = "Wallet";

            if (page < 1)
            {
                page = 1;
            }

            var userId = _currentUser.UserId;
            var balance = await _profileService.GetUserBalanceAsync(userId);
            var transactions = await _profileService.GetUserTransactionsAsync(userId, page, PageSize);

            ViewBag.Balance = balance;
            ViewBag.Transactions = transactions;
            ViewBag.CurrentPage = page;
            ViewBag.HasNextPage = transactions.Count == PageSize;

            return this.View();
        }
    }
}
