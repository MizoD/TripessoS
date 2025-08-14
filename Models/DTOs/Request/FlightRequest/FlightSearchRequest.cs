namespace Models.DTOs.Request.HotelRequest
{
    public class FlightSearchRequest
    {
        public string? Country { get; set; }
        public DateTime? TravelDate { get; set; }
        public int NumberOfPassengers { get; set; }

    }
}
