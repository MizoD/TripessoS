using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? TripId { get; set; }
        public Trip? Trip { get; set; }

        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        public int? HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
        public ICollection<WishlistItem>? Items { get; set; }

    }
}
