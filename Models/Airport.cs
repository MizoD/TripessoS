using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Airport
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public int CountryId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public Country Country { get; set; } = null!;
        public ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();
        public ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();
    }
}
