using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.TripResponse
{
    public class TripDetailsResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CountryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int DurationDays { get; set; }
        public bool IsAvailable { get; set; }

        public List<TripResponse>? RelatedTrips { get; set; }
        public List<ReviewResponse>? Reviews { get; set; }
    }
}
