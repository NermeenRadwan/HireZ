//extensions to read current user id

using System.Security.Claims;

namespace HireZ.Utilities
{
    public static class ClaimsExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user?.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null) return null;
            return int.TryParse(idClaim.Value, out var id) ? id : null;
        }
    }
}
