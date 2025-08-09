using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.HotelResponse
{
    public class RoomListResponse
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int HotelId { get; set; }
        public string? RoomName { get; set; } 
        public int Capacity { get; set; }
        public int AvailableRooms { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable => AvailableRooms > 0;
        public List<string>? Amenities { get; set; } = new();
    }
}
