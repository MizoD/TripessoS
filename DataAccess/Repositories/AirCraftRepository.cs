using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class AirCraftRepository : Repository<AirCraft>, IAirCraftRepository
    {
        public AirCraftRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
