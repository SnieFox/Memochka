using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memochka.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser<User> _userServices;
        private readonly MemochkaContext _context;

        public UserController(IUser<User> userServices, MemochkaContext context)
        {
            _userServices = userServices;
            _context = context;
        }

        public IActionResult Login() => View();
        public IActionResult Registration() => View();
        [Authorize]
        public async Task<IActionResult> ProfilePage()
        {
            var userIdentity = HttpContext.User.Identity.Name;
            var user = await _context.Users
                .Include(r => r.Role)
                .Where(u => u.Login == userIdentity)
                .FirstOrDefaultAsync();

            return View(user);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPanel()
        {
            var userIdentity = HttpContext.User.Identity.Name;
            var user = await _context.Users
                .Include(r => r.Role)
                .Where(u => u.Login == userIdentity)
                .FirstOrDefaultAsync();

            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MainPage", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {

            var loginUser = await _userServices.LoginUserAsync(user, HttpContext);
            if (!loginUser.Succeeded)
            {
                foreach (var error in loginUser.Errors)
                {
                    ModelState.AddModelError("Login", error.Description);
                }
                return View(user);
            }
            return RedirectToAction("MainPage", "Home");
        }
        [HttpPost]

        public async Task<IActionResult> Registration(User user)
        {
            var createUser = await _userServices.ValidateUserAsync(user);
            var loginUser = await _userServices.LoginUserAsync(user, HttpContext);
            if (!createUser.Succeeded)
            {
                string errorMessge = string.Empty;
                foreach (var error in createUser.Errors)
                {
                    if (error.Description.ToLower().Contains("логін"))
                        ModelState.AddModelError("Login", error.Description);
                    if (error.Description.ToLower().Contains("нікнейм"))
                        ModelState.AddModelError("Nickname", error.Description);
                }
                return View(user);
            }
            if (!loginUser.Succeeded)
            {
                foreach (var error in loginUser.Errors)
                {
                    ModelState.AddModelError("Login", error.Description);
                }
                return View(user);
            }
            return RedirectToAction("MainPage","Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUserData(User user)
        {
            var updateUser = await _userServices.UpdateUserAsync(user);
            if (!updateUser.IsSuccess)
                return BadRequest(updateUser.ErrorMessage);
            return RedirectToAction("ProfilePage", user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetProfilePicture(User user)
        {
            if (!Request.Form.Files.Any())
                return RedirectToAction("ProfilePage", user);
            var changeProfilePicture = await _userServices.ChangeProfilePictureAsync(user.Id, Request.Form.Files[0]);
            if (!changeProfilePicture.IsSuccess)
                return BadRequest(changeProfilePicture.ErrorMessage);
            return RedirectToAction("ProfilePage", user);
        }
    }
}
