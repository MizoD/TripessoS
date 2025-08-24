using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public enum Coach { Economy, Business, FirstClass}
    public class Seat
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Seat number is required")]
        [StringLength(10, ErrorMessage = "Seat number cannot exceed 10 characters")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seat class is required")]
        public Coach Coach { get; set; }

        public bool IsBooked { get; set; }
        public bool IsCheckedIn { get; set; }

        [Required(ErrorMessage = "Flight is required")]
        public int FlightId { get; set; }
        [JsonIgnore]
        public Flight Flight { get; set; } = null!;

        [NotMapped]
        public string SeatLabel => $"{Number} ({Coach})";

        [NotMapped]
        public decimal CurrentPrice => Flight?.GetPriceForCoach(Coach) ?? 0;

        public bool CanCheckIn()
        {
            return IsBooked && !IsCheckedIn &&
                   Flight.Status == FlightStatus.Boarding;
        }
    }
}
