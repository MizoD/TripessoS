using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string? Name { get; set; }

        public ICollection<Trip>? Trips { get; set; }
        public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    }
}
