namespace Models.DTOs.Request.TripRequest
{
    public class TripSearchRequest
    {
        public string? CountryName { get; set; }
        public DateTime? DesiredDate { get; set; }
        public int NumberOfPassengers { get; set; }
    }
}
