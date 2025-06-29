using GraphQL.Data;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace GraphQL
{
    public static class ClaimsPrincipalExtension
    {
        public static int GetUserId(this ClaimsPrincipal claims) => int.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier));

        public static string GetUserRole(this ClaimsPrincipal claims) => claims.FindFirstValue(ClaimTypes.Role);
    }
}
