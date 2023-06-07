using Memochka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemochkaContext _context;
        public HomeController(ILogger<HomeController> logger, MemochkaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult MainPage() => View();
        public IActionResult Articles() => View();
        public IActionResult Memes() => View();
        public IActionResult MemesOffer() => View();
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var userAuth = _context.Users.FirstOrDefault(u=>
                u.Login==user.Login&&u.Password==user.Password);
            if (userAuth == null) return RedirectToAction("Login");
            var userRole = _context.Users
                .Include(u => u.Role)
                .Where(u=>u.Role.Id==u.RoleId)
                .Select(r => r.Role.Roles)
                .FirstOrDefault();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(ClaimTypes.Name, userAuth.Login),
                        new(ClaimTypes.Role, userRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme)));
            return RedirectToAction("MainPage");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MainPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}