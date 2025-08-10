namespace Models
{
    public enum PaymentMethod { Visa, CashOnSite}
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
