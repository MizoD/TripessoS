namespace Models.DTOs.Response.TripResponse
{
    public class PaginatedTripResponse
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public IEnumerable<TripResponse> Trips { get; set; } = new List<TripResponse>();
    }
}
