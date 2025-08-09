using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.HotelResponse
{
     public class RoomDetailsResponse
    {
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public int Capacity { get; set; }
        public int AvailableRooms { get; set; }
        public decimal PricePerNight { get; set; }
        public List<string> Photos { get; set; } = new();
        public List<string> Amenities { get; set; } = new();

    }
}
