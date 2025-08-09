using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? TripId { get; set; }
        public Trip? Trip { get; set; }
        public int NumberOfPassengers { get; set; }


        public int? HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
        public ICollection<CartItem>? Items { get; set; }

    }
}
