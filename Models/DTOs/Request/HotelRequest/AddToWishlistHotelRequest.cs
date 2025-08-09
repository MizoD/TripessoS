using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.HotelRequest
{
    public class AddToWishlistHotelRequest
    {
        [Required(ErrorMessage = "HotelId is required.")]
        public int HotelId { get; set; }
    }
}
