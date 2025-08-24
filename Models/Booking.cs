using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public enum PaymentMethod { Visa, CASHONSITE}
    public enum BookingStatus
    {
        Pending,    
        Failed,  
        Paid,       
        Cancelled,
        Refunded
    }
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal TotalAmount { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentId { get; set; }
        public string? SessionId { get; set; }
        public int Tickets { get; set; }
        public DateTime? IssuedAt { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [JsonIgnore]
        public ApplicationUser User { get; set; } = null!;

        public int? TripId { get; set; }
        [JsonIgnore]
        public Trip? Trip { get; set; }
        public int? FlightId { get; set; }
        [JsonIgnore]
        public Flight? Flight { get; set; }
        public int? HotelId { get; set; }
        [JsonIgnore]
        public Hotel? Hotel { get; set; }
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    }
}
