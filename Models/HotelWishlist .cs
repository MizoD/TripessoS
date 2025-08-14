using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(HotelId))]
    public class HotelWishlist
    {
        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
