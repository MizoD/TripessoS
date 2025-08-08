namespace Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public Seat Seat { get; set; } = null!;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public bool IsCheckedIn { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;

    }
}
