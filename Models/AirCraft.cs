using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public enum AirCraftStatus { Ready, Busy, Maintainance}
    public enum AirCraftType { Economy, Business, Private}
    public class AirCraft
    {
        public int Id { get; set; }
        [Required]
        public string Model { get; set; } = null!;
        [Required]
        public int Capacity { get; set; }
        public AirCraftStatus Status { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public AirCraftType Type { get; set; }
        public int AirportId { get; set; }
        [JsonIgnore]
        public Airport Airport { get; set; } = null!;
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
