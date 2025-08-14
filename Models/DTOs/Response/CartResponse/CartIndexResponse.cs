
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.CartResponse
{
    public class CartIndexResponse
    {
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Price);
    }
}
