namespace Models.DTOs.Response.HotelResponse
{
    public class HotelResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int AvailableRooms { get; set; } 
        public decimal PricePerNight { get; set; }
        public string City { get; set; } = null!;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = "";
        public int? TripId { get; set; }
        public string? TripTitle { get; set; }
        public int BookingCount { get; set; }     // ✅ إضافة للعرض فقط
    }
}
