using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
