namespace Models.DTOs.Response.HotelResponse
{
    public class HotelSerachResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<Hotel> Data { get; set; } = new();

    }
}
