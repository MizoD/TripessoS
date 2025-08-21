using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(HotelId))]
    public class HotelWishlist
    {
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
