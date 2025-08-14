using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request.HotelRequest
{
    public class HotelBookingRequest
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be at least 1")]
        public int NumberOfGuests { get; set; }

        public string? SpecialRequests { get; set; }

    }
}
