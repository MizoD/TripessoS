using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(SeatId), nameof(FlightId))]
    public class FlightSeat
    {
        [Required]
        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;
        [Required]
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
    }
}
