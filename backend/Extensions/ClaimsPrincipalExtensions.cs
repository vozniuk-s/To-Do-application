using System.Security.Claims;

namespace backend.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var stringId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(stringId) || !int.TryParse(stringId, out int id))
                throw new UnauthorizedAccessException("Unauthorized");

            return id;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if(string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Unauthorized");

            return role;
        }
    }
}
