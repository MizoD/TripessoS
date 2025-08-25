namespace Models.DTOs.Response
{
    public class CartItemResponse
    {
        public string Type { get; set; } = "";
        public int ItemId { get; set; }
        public string Title { get; set; } = "";
        public int PassengersOrRooms { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }

    }
}
