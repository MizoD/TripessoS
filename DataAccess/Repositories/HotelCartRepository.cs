using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class HotelCartRepository : Repository<HotelCart>, IHotelCartRepository
    {
        public HotelCartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
