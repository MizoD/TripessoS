using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        public FlightRepository(ApplicationDbContext context) : base(context)
        {
            
        }

    }
}
