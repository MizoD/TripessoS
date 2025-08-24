using System.Text.Json.Serialization;

namespace Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public int BookingId { get; set; }
        [JsonIgnore]
        public Booking Booking { get; set; } = null!;

        public int? SeatId { get; set; }
        public Seat? Seat { get; set; }
    }
}
