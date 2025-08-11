using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum FlightStatus { OnGround, TakingOFF, OnAir, Landing}
    public class Flight
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public FlightStatus Status { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int DepartureAirportId { get; set; }
        public Airport DepartureAirport { get; set; } = null!;
        public int ArrivalAirportId { get; set; }
        public Airport ArrivalAirport { get; set; } = null!;
        public int AirCraftId { get; set; }
        public AirCraft Aircraft { get; set; } = null!;
        public int TripId { get; set; }
        public Trip? Trip { get; set; }
        public ICollection<FlightSeat> Seats { get; set; } = new List<FlightSeat>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
