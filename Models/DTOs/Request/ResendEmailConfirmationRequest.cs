using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
    }
}
