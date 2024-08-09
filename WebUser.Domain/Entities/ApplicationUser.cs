using Microsoft.AspNetCore.Identity;

namespace WebUser.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsActive { get; set; }
    }
}
