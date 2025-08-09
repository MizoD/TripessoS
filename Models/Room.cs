using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Room
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Price { get; set; }

        [Required]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        [Required, Range(1, 20)]
        public int Capacity { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int AvailableRooms { get; set; }
        public bool IsAvailable { get; set; }

        [Required, Range(0.0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        // Navigation Collections
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
