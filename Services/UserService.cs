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
    public class UserService : ICreateUser<User>
    {
        private readonly MemochkaContext _context;
        public UserService(MemochkaContext context) => _context = context;

        public Task<IdentityResult> ValidateAsync(User user)
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

    }
}
