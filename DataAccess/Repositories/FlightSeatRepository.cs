using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class FlightSeatRepository : Repository<FlightSeat>, IFlightSeatRepository
    {
        public FlightSeatRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
