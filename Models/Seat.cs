using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum Coach { Economy, Business, FirstClass}
    public class Seat
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; } = null!;
        public Coach Coach { get; set; }

        public bool IsBooked { get; set; }
        public bool IsCheckedIn { get; set; }
        public ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();

    }
}
