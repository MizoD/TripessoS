using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class AirportRepository : Repository<Airport>, IAirportRepository
    {
        public AirportRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
