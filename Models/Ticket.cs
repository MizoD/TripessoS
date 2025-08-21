using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

    }
}
