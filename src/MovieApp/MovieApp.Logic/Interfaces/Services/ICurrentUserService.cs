namespace MovieApp.Logic.Interfaces.Services
{
    /// <summary>
    /// Provides the identity of the currently authenticated user.
    /// Implemented differently per host (WebApi reads JWT claims, MVC reads token store).
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>Gets the ID of the currently authenticated user.</summary>
        int UserId { get; }
    }
}
