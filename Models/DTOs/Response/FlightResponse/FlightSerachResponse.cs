namespace Models.DTOs.Response.HotelResponse
{
    public class FlightSerachResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<Flight> Data { get; set; } = new();

    }
}
