using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
