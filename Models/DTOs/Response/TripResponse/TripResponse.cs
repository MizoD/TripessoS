namespace Models.DTOs.Response.TripResponse
{
    public class TripResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TripType TripType { get; set; }
        public string CountryName { get; set; } = null!;
        public int CountryId { get; set; }
        public string? ImageUrl { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public List<string>? SecondaryImages { get; set; }
        public string? VideoUrl { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
