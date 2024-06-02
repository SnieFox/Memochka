using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Memochka.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Memochka.Services
{
    public class UserService : IUser<User>
    {
        private readonly MemochkaContext _context;
        public UserService(MemochkaContext context) => _context = context;

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(User user)
        {
            try
            {
                var userDb = await _context.Users
                    .FindAsync(user.Id);
                if (userDb == null)
                    return (false, "Current user does not exist");
                if (user.Nickname != userDb.Nickname)
                {
                    if (_context.Users.Any(n => n.Nickname == user.Nickname))
                        return (false, "Username already exists");
                }
                user = user with
                {
                    Id = userDb.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Login = userDb.Login,
                    Nickname = userDb.Nickname,
                    Password = userDb.Password,
                    RoleId = userDb.RoleId
                };
                _context.Entry(userDb).CurrentValues.SetValues(user);
                var saved = await _context.SaveChangesAsync();
                return saved == 0
                    ? (false, "Something went wrong when saving data to db")
                    : (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false,e.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> ChangeProfilePictureAsync(int userId, IFormFile profilePicture)
        {
            try
            {
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "users",
                    $"{userId}.jpg"
                );
                using var stream = new FileStream(path, FileMode.Create);
                await profilePicture.CopyToAsync(stream);
                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

    }
}
