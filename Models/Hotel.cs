using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ThumbnailImageUrl { get; set; }

        // Location / Country
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public int? TripId { get; set; }
        public Trip? Trip { get; set; }

        // Navigation Collections
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
