using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.HotelResponse
{
    public class HotelDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Rating { get; set; }
        public List<string> Amenities { get; set; } = new();
        public List<string> Photos { get; set; } = new();
        public List<Models.DTOs.Response.HotelResponse.RoomListResponse> Rooms { get; set; } = new();
        public List<HotelListResponse> RelatedHotels { get; set; } = new();
        public List<Models.DTOs.Response.TripResponse.ReviewResponse> Reviews { get; set; } = new();
    }
}
