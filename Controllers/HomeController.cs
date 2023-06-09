using Memochka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemochkaContext _context;
        private readonly ICreateUser<User> _createUser;
        public HomeController(ILogger<HomeController> logger, MemochkaContext context, ICreateUser<User> createUser)
        {
            _createUser = createUser;
            _logger = logger;
            _context = context;
        }

        public IActionResult MainPage() => View();
        public IActionResult Articles() => View();
        public IActionResult Memes() => View();
        public IActionResult MemesOffer() => View();
        public IActionResult Login() => View();
        public IActionResult Registration() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {

            AuthorizeUserAsync(user);
            return RedirectToAction("MainPage");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MainPage");
        }

        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            var createUser = await _createUser.ValidateAsync(user);
            if (!createUser.Succeeded)
            {
                string errorMessge = string.Empty;
                foreach (var error in createUser.Errors)
                {
                    if(error.Description.ToLower().Contains("логін"))
                        ModelState.AddModelError("Login", error.Description);
                    if (error.Description.ToLower().Contains("нікнейм"))
                        ModelState.AddModelError("Nickname", error.Description);
                }
                return View(user);
            }
            AuthorizeUserAsync(user);
            return RedirectToAction("MainPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        async Task<IdentityResult> AuthorizeUserAsync(User user)
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
                var userRole = _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.Id == u.RoleId)
                    .Select(r => r.Role.Roles)
                    .FirstOrDefault();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
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