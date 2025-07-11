using System.Security.Claims;


namespace Lumina.Api.ASP.helpers
{
    public static class UserHelper
    {
        public static bool IsUserLoggedIn(ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier) != null;
        }

        public static bool TryGetUserId(ClaimsPrincipal user, out string userId)
        {
            userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            return !string.IsNullOrEmpty(userId);
        }
    }
}
