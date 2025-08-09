using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.HotelRequest
{
    public class RoomBookingRequest
    {
        [Required(ErrorMessage = "HotelId is required.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "RoomId is required.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "CheckIn is required.")]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "CheckOut is required.")]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }

        [Range(1, 10)]
        public int RoomQuantity { get; set; } = 1;
    }
}
