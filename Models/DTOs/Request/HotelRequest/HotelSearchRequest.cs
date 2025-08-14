using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request.HotelRequest
{
    public class HotelSearchRequest
    {
        public string? Country { get; set; }
        public DateTime? CheckInDate { get; set; }
        public int NumberOfGuests { get; set; }

    }
}
