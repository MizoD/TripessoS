using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class FlightWishlistRepository : Repository<FlightWishlist>, IFlightWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightWishlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Get all wishlist items with related flights + airports
        public async Task<IEnumerable<FlightWishlist>> GetAllWishlistItemsAsync()
        {
            return await _context.FlightWishlists
                .Include(w => w.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                .Include(w => w.Flight.ArrivalAirport)
                .ToListAsync();
        }

        public Task GetFirstOrDefaultAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        // ✅ Get single wishlist item by Id
        public async Task<FlightWishlist?> GetWishlistItemByIdAsync(int id)
        {
            return await _context.FlightWishlists
                .Include(w => w.Flight)
                .FirstOrDefaultAsync(w => w.FlightId == id);
        }
    }
}
