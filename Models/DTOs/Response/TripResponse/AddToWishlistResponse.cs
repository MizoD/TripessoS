using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.TripResponse
{
    public class AddToWishlistResponse
    {
        public int WishlistId { get; set; }
        public int TripId { get; set; }
        public string? TripTitle { get; set; }
        public string? CountryName { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
