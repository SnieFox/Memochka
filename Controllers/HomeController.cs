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
        private readonly IUser<User> _userServices;
        public HomeController(ILogger<HomeController> logger, MemochkaContext context, IUser<User> userServices)
        {
            _userServices = userServices;
            _logger = logger;
            _context = context;
        }

        public IActionResult MainPage() => View();
        public IActionResult Articles() => View();
        public IActionResult Memes() => View();
        public IActionResult MemesOffer() => View();
        public IActionResult Login() => View();
        public IActionResult Registration() => View();

        public IActionResult ProfilePage()
        {
            var userIdentity = HttpContext.User.Identity.Name;
            var user = _context.Users
                .Include(r=>r.Role)
                .Where(u => u.Login == userIdentity)
                .FirstOrDefault();
            
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("MainPage");
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
            return RedirectToAction("MainPage");
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
                    if(error.Description.ToLower().Contains("логін"))
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
            return RedirectToAction("MainPage");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserData(User user)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errorMessage = ModelState.Values
            //        .SelectMany(e => e.Errors)
            //        .Select(e => e.ErrorMessage)
            //        .ToList();
            //    return BadRequest(errorMessage);
            //}
            var updateUser = await _userServices.UpdateUserAsync(user);
            if (!updateUser.IsSuccess)
                return BadRequest(updateUser.ErrorMessage);
            return RedirectToAction("ProfilePage", user);
        }

        [HttpPost]
        public async Task<IActionResult> SetProfilePicture(User user)
        {
            if (!Request.Form.Files.Any())
                return RedirectToAction("ProfilePage", user);
            var changeProfilePicture = await _userServices.ChangeProfilePictureAsync(user.Id, Request.Form.Files[0]);
            if(!changeProfilePicture.IsSuccess)
                return BadRequest(changeProfilePicture.ErrorMessage);
            return RedirectToAction("ProfilePage", user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}