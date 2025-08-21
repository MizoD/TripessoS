using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(TripId))]
    public class TripCart : Cart
    {
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
