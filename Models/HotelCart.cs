using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [PrimaryKey(nameof(UserId), nameof(HotelId))]
    public class HotelCart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        [Required, Range(1, int.MaxValue, ErrorMessage = "Must book at least 1 passenger.")]
        public int NumberOfPassengers { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
