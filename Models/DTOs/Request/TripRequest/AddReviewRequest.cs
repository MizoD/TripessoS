using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.TripRequest
{
    public class AddReviewRequest
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment can't exceed 1000 characters.")]
        public string? Comment { get; set; }
    }
}
