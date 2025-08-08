using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        public SeatRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
