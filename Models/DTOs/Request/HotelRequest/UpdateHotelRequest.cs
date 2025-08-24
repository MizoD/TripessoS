using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request.HotelRequest
{
    public class UpdateHotelRequest
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int AvailableRooms { get; set; } 

        [Required]
        [Range(0, 999999)]
        public decimal PricePerNight { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = null!;

        [Required]
        public int CountryId { get; set; }

        public int? TripId { get; set; }
    }
}
