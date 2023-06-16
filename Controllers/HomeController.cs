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
using Memochka.Services.Interfaces;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemochkaContext _context;
        private readonly IUser<User> _userServices;
        private readonly IMeme _memeService;
        private readonly IArticle _articleService;
        public HomeController(ILogger<HomeController> logger, MemochkaContext context, IUser<User> userServices, IMeme memeService, IArticle articleService)
        {
            _articleService = articleService;
            _memeService = memeService;
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
        public IActionResult CreateMemePage() => View();
        public IActionResult CreateArticlePage() => View();

        public IActionResult ProfilePage()
        {
            var userIdentity = HttpContext.User.Identity.Name;
            var user = _context.Users
                .Include(r=>r.Role)
                .Where(u => u.Login == userIdentity)
                .FirstOrDefault();
            
            return View(user);
        }

        #region User

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

        #endregion

        #region Meme

        public async Task<IActionResult> CreateMeme(Meme meme)
        {
            var userLogin = HttpContext.User.Identity.Name;
            if (!Request.Form.Files.Any())
                return RedirectToAction("CreateMemePage", meme);
            var memeService = await _memeService.CreateMemeAsync(userLogin, meme, Request.Form.Files);
            if (!memeService.IsSuccess)
                return BadRequest(memeService.ErrorMessage);
            return RedirectToAction("ProfilePage");
        }

        public async Task<IActionResult> MemePage(int id)
        {
            var memeViews = await _memeService.UpMemeViewsAsync(id);
            if(!memeViews.IsSuccess)
                return BadRequest(memeViews.ErrorMessage);
            var meme = _context.Memes
                .Where(m => m.Id == id)
                .Include(u=>u.User)
                .Include(u => u.MemePictures)
                .FirstOrDefault();
            if (meme == null)
                return NotFound();
            return View(meme);
        }

        #endregion

        #region Article

        public async Task<IActionResult> CreateArticle(Article article)
        {
            //bool isArticleParagraph = false;
            //var creteArticle = await _articleService.CreateArticleAsync(article);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddArticleParagraph([FromForm]Article article)
        {
            bool isArticleParagraph = true;
            var userLogin = HttpContext.User.Identity.Name;
            if (!Request.Form.Files.Any())
                return RedirectToAction("CreateArticlePage", article);
            var creteArticleParagraph = await _articleService.CreateArticleAsync(article, userLogin, Request.Form.Files[0]);
            if (!creteArticleParagraph.IsSuccess)
                return BadRequest(creteArticleParagraph.ErrorMessage);
            var returnArticle = await _context.Articles
                .Include(a => a.ArticleParagraphs)
                .Where(a => a.Title == article.Title).FirstOrDefaultAsync();
            return RedirectToAction("CreateArticlePage",returnArticle);
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}