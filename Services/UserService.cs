using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Memochka.Services
{
    public class UserService : IUser<User>
    {
        private readonly MemochkaContext _context;
        public UserService(MemochkaContext context) => _context = context;

        public Task<IdentityResult> ValidateUserAsync(User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (_context.Users.Any(u => u.Login == user.Login))
                errors.Add(new IdentityError
                {
                    Description = "Логін зайнятий."
                });
            if (_context.Users.Any(u => u.Nickname == user.Nickname))
                errors.Add(new IdentityError
                {
                    Description = "Нікнейм зайнятий."
                });
            if (errors.Count == 0)
            {
                var defaaultRole = _context.Roles.Where(r => r.Roles == "User").Select(r => r.Id).FirstOrDefault();
                _context.Users.Add(user with { RoleId = defaaultRole });
                _context.SaveChanges();
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        public async Task<IdentityResult> LoginUserAsync(User user, HttpContext context)
        {
            List<IdentityError> errors = new List<IdentityError>();

            var userAuth = _context.Users.FirstOrDefault(u =>
                u.Login == user.Login && u.Password == user.Password);
            if (userAuth == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Неправильні дані."
                });
            }
            if (errors.Count == 0)
            {
                var userRole = _context.Roles
                    .Where(u => u.Id== userAuth.RoleId)
                    .Select(r=>r.Roles).FirstOrDefault();
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim>
                        {
                            new(ClaimTypes.Name, userAuth.Login),
                            new(ClaimTypes.Role, userRole)
                        }, CookieAuthenticationDefaults.AuthenticationScheme)));
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(errors.ToArray());
        }

    }
}
