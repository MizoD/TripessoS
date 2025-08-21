using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(FlightId))]
    public class FlightCart : Cart
    {
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
    }
}
