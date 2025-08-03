using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ImgUrl { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime RegistraionDate { get; set; }
    }
}
