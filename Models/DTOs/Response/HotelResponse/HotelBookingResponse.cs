using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.HotelResponse
{
    public class HotelBookingResponse
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? UserName { get; set; }

        public int HotelId { get; set; }
        public string? HotelName { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
