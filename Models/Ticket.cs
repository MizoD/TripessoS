using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int BookingId { get; set; }
        [JsonIgnore]
        public Booking Booking { get; set; } = null!;

    }
}
