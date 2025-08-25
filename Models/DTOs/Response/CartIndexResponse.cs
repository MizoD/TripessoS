namespace Models.DTOs.Response
{
    public class CartIndexResponse
    {
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Price);
    }
}
