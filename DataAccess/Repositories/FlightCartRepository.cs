using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class FlightCartRepository : Repository<FlightCart>, IFlightCartRepository
    {
        public FlightCartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
