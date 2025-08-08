namespace Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public int ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    }
}
