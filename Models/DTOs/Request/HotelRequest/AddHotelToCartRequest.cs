using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.HotelRequest
{
    public class AddHotelToCartRequest
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int Quantity { get; set; }
    }
}
