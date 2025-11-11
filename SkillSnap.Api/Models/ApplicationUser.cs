using Microsoft.AspNetCore.Identity;

namespace SkillSnap.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom properties here if needed
        public string? FullName { get; set; }
    }
}
