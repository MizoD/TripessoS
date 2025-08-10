using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Must book at least 1 passenger.")]
        public int NumberOfPassengers { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public static implicit operator Cart(Cart v)
        {
            throw new NotImplementedException();
        }
    }
}
