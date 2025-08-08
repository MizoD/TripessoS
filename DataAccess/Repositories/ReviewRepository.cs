using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
