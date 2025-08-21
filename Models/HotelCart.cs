using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(HotelId))]
    public class HotelCart : Cart
    {
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
    }
}
