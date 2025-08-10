using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class TripWishlist
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
