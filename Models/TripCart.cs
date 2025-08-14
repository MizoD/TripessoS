using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(TripId))]
    public class TripCart
    {
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        [Required, Range(1, int.MaxValue, ErrorMessage = "Must book at least 1 passenger.")]
        public int NumberOfPassengers { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
