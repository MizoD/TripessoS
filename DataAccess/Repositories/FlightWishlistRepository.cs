using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class FlightWishlistRepository : Repository<FlightWishlist> , IFlightWishlistRepository
    {
        public FlightWishlistRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
