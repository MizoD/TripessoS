using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request
{
    public class PayCartRequest
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
