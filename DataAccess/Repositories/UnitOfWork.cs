using DataAccess.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Models;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(IApplicationUserOTPRepository applicationUserOTPRepository,ApplicationDbContext dbContext, 
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            ApplicationUserOTPRepository = applicationUserOTPRepository;
            this.dbContext = dbContext;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }
        public void Dispose()
        {
            dbContext.Dispose();
        }

    }
}
