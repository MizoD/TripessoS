using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
