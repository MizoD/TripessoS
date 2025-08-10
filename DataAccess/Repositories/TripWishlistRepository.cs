using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class TripWishlistRepository : Repository<TripWishlist> , ITripWishlistRepository
    {
        public TripWishlistRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
