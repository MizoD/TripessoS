using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.TripResponse
{
    public class AddToCartResponse
    {
        public int CartId { get; set; }
        public int TripId { get; set; }
        public string? TripTitle { get; set; }
        public int NumberOfPassengers { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
