﻿using Memochka.Models;
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
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Memes(string category, int year)
        {
            var memes = await _memeService.GetOrderedemesAsync(category, year);
            if (!memes.IsSuccess)
                return BadRequest(memes.ErrorMessage);
            return View(memes.MemesList);
        }
        public IActionResult MemesOffer() => View();
        public IActionResult Login() => View();
        public IActionResult Registration() => View();

        [Authorize]
        public IActionResult CreateMemePage() => View();

        [Authorize]
        public IActionResult CreateArticlePage() => View();

        [Authorize]
        public async Task<IActionResult> ProfilePage()
        {
            var userIdentity = HttpContext.User.Identity.Name;
            var user = await _context.Users
                .Include(r=>r.Role)
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

        #region User
        [Authorize]
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
            if(!changeProfilePicture.IsSuccess)
                return BadRequest(changeProfilePicture.ErrorMessage);
            return RedirectToAction("ProfilePage", user);
        }

        #endregion

        #region Meme
        [Authorize]
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
            Meme meme = new Meme();
            if (User.IsInRole("Admin"))
            {
                meme = _context.Memes
                    .Where(m => m.Id == id)
                    .Include(u => u.User)
                    .Include(u => u.MemePictures)
                    .FirstOrDefault();
            }
            else if (User.IsInRole("User"))
            {
                meme = _context.Memes
                    .Where(m => m.Id == id && m.IsApproved==true)
                    .Include(u => u.User)
                    .Include(u => u.MemePictures)
                    .FirstOrDefault();
            }
            if (meme == null)
                return NotFound("The meme does not exist or has not been approved by the moderator");
            return View(meme);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PublishMeme(int id)
        {
            var publishedMeme = await _memeService.PublishMeme(id);
            if (!publishedMeme.IsSuccess)
                return BadRequest(publishedMeme.ErrorMessage);
            return RedirectToAction("AdminPanel");
        }

        #endregion

        #region Article

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle(Article article)
        {
            var userLogin = HttpContext.User.Identity.Name;
            if (!Request.Form.Files.Any())
                return RedirectToAction("CreateArticlePage", article);
            var creteArticleParagraph = await _articleService.CreateArticleAsync(article, userLogin, Request.Form.Files);
            if (!creteArticleParagraph.IsSuccess)
                return BadRequest(creteArticleParagraph.ErrorMessage);
            return RedirectToAction("ProfilePage");
        }

        public async Task<IActionResult> ArticlePage(int id)
        {
            var articleViews = await _articleService.UpArticleViewsAsync(id);
            if (!articleViews.IsSuccess)
                return BadRequest(articleViews.ErrorMessage);
            Article article = new Article();
            if (User.IsInRole("Admin"))
            {
                article = _context.Articles
                    .Where(a => a.Id == id)
                    .Include(u => u.User)
                    .Include(u => u.ArticleParagraphs)
                    .FirstOrDefault();
            }
            else if(User.IsInRole("User"))
            {
                article = _context.Articles
                    .Where(a => a.Id == id && a.IsApproved==true)
                    .Include(u => u.User)
                    .Include(u => u.ArticleParagraphs)
                    .FirstOrDefault();
            }
            if (article == null)
                return NotFound("The article does not exist or has not been approved by the moderator");
            return View(article);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PublishArticle(int id)
        {
            var publishedArticle = await _articleService.PublishArticle(id);
            if (!publishedArticle.IsSuccess)
                return BadRequest(publishedArticle.ErrorMessage);
            return RedirectToAction("AdminPanel");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}