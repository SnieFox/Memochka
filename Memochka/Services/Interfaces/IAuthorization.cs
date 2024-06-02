using Memochka.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Memochka.Services.Interfaces
{
    public interface IAuthorization
    {
        Task<IdentityResult> ValidateUserAsync(User user);
        Task<IdentityResult> LoginUserAsync(User user, HttpContext context);
    }
}
