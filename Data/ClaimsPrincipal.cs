using System.Linq;

namespace Lib
{
    public static class ClaimsPrincipal 
    {
        public static string Username(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value;
        } 
    }
}