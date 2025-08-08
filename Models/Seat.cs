namespace Models
{
    public enum Coach { Economy, Business, FirstClass}
    public class Seat
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public Coach Coach { get; set; }
        public bool IsBooked { get; set; }
        public bool IsCheckedIn { get; set; }
        public int AirCraftId { get; set; }
        public AirCraft AirCraft { get; set; } = null!;
    }
}
