using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Hotel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;
        [Required]
        public int AvailableRooms { get; set; }
        [Required]
        public decimal PricePerNight { get; set; }
        [Required]
        public string City { get; set; } = null!;
        public int Traffic { get; set; }
        [Required]
        public int CountryId { get; set; }
        [JsonIgnore]
        public Country Country { get; set; } = null!;
        public int? TripId { get; set; }
        [JsonIgnore]
        public Trip? Trip { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
