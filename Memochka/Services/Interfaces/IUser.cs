using Microsoft.AspNetCore.Identity;

namespace Memochka.Services.Interfaces
{
    public interface IUser<TUser> where TUser : class
    {
        Task<IdentityResult> ValidateUserAsync(TUser user);
        Task<IdentityResult> LoginUserAsync(TUser user, HttpContext context);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(TUser user);
        Task<(bool IsSuccess, string ErrorMessage)> ChangeProfilePictureAsync(int userId, IFormFile profilePicture);
    }
}
