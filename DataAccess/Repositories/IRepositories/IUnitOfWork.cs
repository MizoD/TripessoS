using Microsoft.AspNetCore.Identity;
using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        Task<bool> CommitAsync();
    }
}
