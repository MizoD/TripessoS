using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TripId { get; set; }
        public Trip? Trip { get; set; }
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        [Required, Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
