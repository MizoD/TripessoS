using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Request.TripRequest
{
    public class TripSearchRequest
    {
        [Required(ErrorMessage = "Country name is required.")]
        public string? CountryName { get; set; }

        [Required(ErrorMessage = "Desired date is required.")]
        public DateTime DesiredDate { get; set; }

        [Required(ErrorMessage = "Passenger count is required.")]
        [Range(1, 20, ErrorMessage = "Passenger count must be between 1 and 20.")]
        public int NumberOfPassengers { get; set; }
    }
}
