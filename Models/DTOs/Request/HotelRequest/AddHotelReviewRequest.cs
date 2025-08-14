using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Request.HotelRequest
{
    public class AddHotelReviewRequest
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

    }
}
