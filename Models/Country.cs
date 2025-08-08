namespace Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public ICollection<Airport> Airports { get; set; } = new List<Airport>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
