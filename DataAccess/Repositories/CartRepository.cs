using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Check if a flight has enough available seats
        public async Task<bool> IsTripAvailableAsync(int flightId, int quantity)
        {
            var flight = await _context.Trips
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
                return false;

            return flight.AvailableSeats >= quantity;
        }

        // Add a new item to the cart
        public async Task AddToCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }
    }
}
