using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services;
using Memochka.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Memochka.Controllers
{
    public class MemeController : Controller
    {
        private readonly IMeme _memeService;
        private readonly MemochkaContext _context;

        public MemeController(IMeme memeService, MemochkaContext context)
        {
            _memeService = memeService;
            _context = context;
        }
        [Authorize]
        public IActionResult CreateMemePage() => View();
        public IActionResult MemesOffer() => View();
        public async Task<IActionResult> Memes(string category, int year)
        {
            var memes = await _memeService.GetOrderedemesAsync(category, year);
            if (!memes.IsSuccess)
                return BadRequest(memes.ErrorMessage);
            return View(memes.MemesList);
        }
        [Authorize]
        public async Task<IActionResult> CreateMeme(Meme meme)
        {
            var userLogin = HttpContext.User.Identity.Name;
            if (!Request.Form.Files.Any())
                return RedirectToAction("CreateMemePage", meme);
            var memeService = await _memeService.CreateMemeAsync(userLogin, meme, Request.Form.Files);
            if (!memeService.IsSuccess)
                return BadRequest(memeService.ErrorMessage);
            return RedirectToAction("ProfilePage", "User");
        }

        public async Task<IActionResult> MemePage(int id)
        {
            var memeViews = await _memeService.UpMemeViewsAsync(id);
            if (!memeViews.IsSuccess)
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
            else
            {
                meme = _context.Memes
                    .Where(m => m.Id == id && m.IsApproved == true)
                    .Include(u => u.User)
                    .Include(u => u.MemePictures)
                    .FirstOrDefault();
            }
            if (meme.Id == 0)
                return NotFound("The meme does not exist or has not been approved by the moderator");
            return View(meme);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PublishMeme(int id)
        {
            var publishedMeme = await _memeService.PublishMeme(id);
            if (!publishedMeme.IsSuccess)
                return BadRequest(publishedMeme.ErrorMessage);
            return RedirectToAction("AdminPanel", "User");
        }
    }
}
