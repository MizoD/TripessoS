namespace Models
{
    public enum FlightStatus { OnGround, TakingOFF, OnAir, Landing}
    public class Flight
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public FlightStatus Status { get; set; }
        public decimal Price { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int DepartureAirportId { get; set; }
        public Airport DepartureAirport { get; set; } = null!;
        public int ArrivalAirportId { get; set; }
        public Airport ArrivalAirport { get; set; } = null!;
        public int AirCraftId { get; set; }
        public AirCraft Aircraft { get; set; } = null!;
        public int TripId { get; set; }
        public Trip? Trip { get; set; }
        public object SearchTerm { get; set; }
        public object FlyingFrom { get; set; }
        public object FlyingTo { get; set; }
        public object Passengers { get; set; }
    }
}
