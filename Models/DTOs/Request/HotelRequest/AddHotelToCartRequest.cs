namespace Models.DTOs.Request.HotelRequest
{
    public class AddHotelToCartRequest
    {
        public int HotelId { get; set; }
        public int Quantity { get; set; }
    }
}
