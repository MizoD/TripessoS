using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [JsonIgnore]
        public ApplicationUser User { get; set; } = null!;
        public int TripId { get; set; }
        [JsonIgnore]
        public Trip Trip { get; set; } = null!;

        [Required, Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
