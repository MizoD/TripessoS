using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.TripRequest
{
    public class AddToCartRequest
    {
        [Required(ErrorMessage = "Trip ID is required.")]
        public int TripId { get; set; }

        [Required(ErrorMessage = "Number of passengers is required.")]
        [Range(1, 20, ErrorMessage = "You must book between 1 and 20 passengers.")]
        public int NumberOfPassengers { get; set; }
    }
}
