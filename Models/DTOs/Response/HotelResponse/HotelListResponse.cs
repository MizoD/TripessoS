using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.HotelResponse
{
    public class HotelListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double Rating { get; set; } 
        public decimal MinPricePerNight { get; set; } 
        public decimal MaxPricePerNight { get; set; } 
        public string? ThumbnailImageUrl { get; set; }
        public bool HasAvailability { get; set; } 
    }
}
