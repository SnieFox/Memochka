using Microsoft.AspNetCore.Identity;

namespace Memochka.Services
{
    public interface IUser<TUser> where TUser : class
    {
        Task<IdentityResult> ValidateUserAsync(TUser user);
        Task<IdentityResult> LoginUserAsync(TUser user, HttpContext context);
    }
}
