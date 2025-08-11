using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Airport
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();
        public ICollection<Flight> DepratureFlights { get; set; } = new List<Flight>();
    }
}
