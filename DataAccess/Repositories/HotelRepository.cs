using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
