namespace Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EventDate { get; set; }
        public string LocationLink { get; set; } =string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public int TripId { get; set; }
        public Trip? Trip { get; set; }
    }
}
