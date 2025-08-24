using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public enum TripType { Tourism, Religion, Adventure, Romantic}
    public class Trip
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string Description { get; set; } = null!;
        [Required]
        public TripType TripType { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }
        [JsonIgnore]
        public Country? Country { get; set; }
        [Url]
        [StringLength(300)]
        public string? ImageUrl { get; set; }
        [NotMapped]
        public int DurationDays => (EndDate - StartDate).Days;

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Total seats are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Available seats are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available seats cannot be negative.")]
        public int AvailableSeats { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999999.")]
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<string>? SecondryImages { get; set; }
        public string? VideoUrl { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
