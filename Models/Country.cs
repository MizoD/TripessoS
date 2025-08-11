using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Country
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Code { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
        public ICollection<Airport> Airports { get; set; } = new List<Airport>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
