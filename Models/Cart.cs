using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Cart
    {
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required, Range(1, int.MaxValue)]
        public int NumberOfPassengers { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
