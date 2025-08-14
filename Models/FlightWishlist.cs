using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(FlightId))]
    public class FlightWishlist
    {
        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
