using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class CountryRepository : Repository<Event>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
