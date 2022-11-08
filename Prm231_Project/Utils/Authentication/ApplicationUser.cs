using Microsoft.AspNetCore.Identity;

namespace Prm231_Project.Utils.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
