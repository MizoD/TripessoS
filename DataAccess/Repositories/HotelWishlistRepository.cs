using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class HotelWishlistRepository : Repository<HotelWishlist> , IHotelWishlistRepository
    {
        public HotelWishlistRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
