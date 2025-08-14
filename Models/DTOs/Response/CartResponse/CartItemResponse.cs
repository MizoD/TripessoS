using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.CartResponse
{
    public class CartItemResponse
    {
        public string Type { get; set; } = "";
        public int ItemId { get; set; }
        public string Title { get; set; } = "";
        public int PassengersOrRooms { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
