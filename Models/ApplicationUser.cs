using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Conference.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? LastName { get; set; }
        public string? UserPic { get; set; }
    }
}
