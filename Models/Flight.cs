using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public enum FlightStatus {
        Scheduled,
        Boarding,
        Departed,
        InAir,
        Landed,
        Cancelled,
        Delayed
    }
    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Flight title is required")]
        [StringLength(100, ErrorMessage = "Flight title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        public FlightStatus Status { get; set; } = FlightStatus.Scheduled;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }
        public int Traffic { get; set; }

        [Required(ErrorMessage = "Departure airport is required")]
        public int DepartureAirportId { get; set; }
        [JsonIgnore]
        public Airport DepartureAirport { get; set; } = null!;

        [Required(ErrorMessage = "Arrival airport is required")]
        public int ArrivalAirportId { get; set; }
        [JsonIgnore]
        public Airport ArrivalAirport { get; set; } = null!;

        [Required(ErrorMessage = "Aircraft is required")]
        public int AirCraftId { get; set; }
        [JsonIgnore]
        public AirCraft Aircraft { get; set; } = null!;
        public int? TripId { get; set; }
        [JsonIgnore]
        public Trip? Trip { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [NotMapped]
        public TimeSpan Duration => ArrivalTime - DepartureTime;

        [NotMapped]
        public int TotalSeats => Seats.Count;

        [NotMapped]
        public int AvailableSeats => Seats.Count(s => !s.IsBooked);

        [NotMapped]
        public int BookedSeats => Seats.Count(s => s.IsBooked);

        public IEnumerable<Seat> GetAvailableSeats(Coach? coach = null)
        {
            return coach == null
                ? Seats.Where(s => !s.IsBooked)
                : Seats.Where(s => !s.IsBooked && s.Coach == coach.Value);
        }

        public decimal GetPriceForCoach(Coach coach)
        {
            return coach switch
            {
                Coach.Economy => BasePrice,
                Coach.Business => BasePrice * 2.5m,
                Coach.FirstClass => BasePrice * 4m,
                _ => BasePrice
            };
        }
    }
}
