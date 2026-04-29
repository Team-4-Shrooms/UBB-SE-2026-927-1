namespace MovieApp.Features.Shared.Models
{
    public static class SessionManager
    {
        public static int CurrentUserID { get; set; } = 1;
        public static decimal CurrentUserBalance { get; set; } = 0;
        public static bool IsLoggedIn => CurrentUserID > 0;
    }
}
