using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
