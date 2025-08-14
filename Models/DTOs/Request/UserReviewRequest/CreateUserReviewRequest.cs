using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.UserReviewRequest
{
    public class CreateUserReviewRequest
    {
        public string UserId { get; set; } = null!;
        public int TripId { get; set; }
        public int HotelId { get; set; }
        public int Rating { get; set; }   
        public string Comment { get; set; } = null!;
    }
}
