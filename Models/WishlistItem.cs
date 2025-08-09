using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class WishlistItem
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int WishlistId { get; set; }
        public Wishlist? Wishlist { get; set; }

        // Hotel Relationship
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        // Date added to wishlist
        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
