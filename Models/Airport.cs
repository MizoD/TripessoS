namespace Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public Event Country { get; set; } = null!;
    }
}
