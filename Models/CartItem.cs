using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // User Relationship
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // Room Relationship
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Quantity of rooms booked
        public int Quantity { get; set; }

        // Date of booking (optional, for availability checks)
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

    }
}
