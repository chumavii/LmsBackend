using Microsoft.AspNetCore.Identity;

namespace LmsApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }
    }

}
