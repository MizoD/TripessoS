using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    internal class PassengerRepository : Repository<Passenger>, IPassengerRepository
    {
        public PassengerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
