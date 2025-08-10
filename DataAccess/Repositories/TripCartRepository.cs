using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class TripCartRepository : Repository<TripCart>, ITripCartRepository
    {
        public TripCartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
