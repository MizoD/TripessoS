using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsCheckedIn { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

    }
}
