using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum PaymentMethod { Visa, CASHONSITE}
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfTickets { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentId { get; set; }
        public string? SessionId { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int TripId { get; set; }
        public Trip? Trip { get; set; }
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
