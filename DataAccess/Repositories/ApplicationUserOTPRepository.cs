
using DataAccess.Repositories.IRepositories;
using Models;

namespace DataAccess.Repositories
{
    public class ApplicationUserOTPRepository : Repository<ApplicationUserOTP>, IApplicationUserOTPRepository
    {
        public ApplicationUserOTPRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
