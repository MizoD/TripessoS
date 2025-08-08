namespace Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
        public List<Flight> Flights { get; set; } = new List<Flight>();
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
