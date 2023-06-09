using Microsoft.AspNetCore.Identity;

namespace Memochka.Services
{
    public interface ICreateUser<TUser> where TUser : class
    {
        Task<IdentityResult> ValidateAsync(TUser user);
    }
}
