using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(FlightId))]
    public class FlightWishlist
    {
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
