using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.TripRequest
{
    public class UpdateTripRequest
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public TripType TripType { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Url]
        [StringLength(300)]
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public ICollection<string>? SecondaryImages { get; set; }
        public string? VideoUrl { get; set; }
    }
}
