using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request
{
    public class EditProfileRequest
    {
        
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;
        
        public string? Address { get; set; }
        public IFormFile? ImageUrl { get; set; } 
    }
}
