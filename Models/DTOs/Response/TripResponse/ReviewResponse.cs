using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.TripResponse
{
    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public int TripId { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
