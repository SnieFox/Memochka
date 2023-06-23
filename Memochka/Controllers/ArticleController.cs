using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memochka.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticle _articleService;
        private readonly MemochkaContext _context;

        public ArticleController(IArticle articleService, MemochkaContext context)
        {
            _articleService = articleService;
            _context = context;
        }

        public IActionResult Articles() => View();
        [Authorize]
        public IActionResult CreateArticlePage() => View();
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle(Article article)
        {
            var userLogin = HttpContext.User.Identity.Name;
            if (!Request.Form.Files.Any())
            {
                ModelState.AddModelError("Title", "Main Image could not be null");
                return RedirectToAction("CreateArticlePage", article);
            }
            var creteArticleParagraph = await _articleService.CreateArticleAsync(article, userLogin, Request.Form.Files);
            if (!creteArticleParagraph.IsSuccess)
            {
                ModelState.AddModelError("Title",creteArticleParagraph.ErrorMessage);
                return RedirectToAction("CreateArticlePage", article);
                //return BadRequest(creteArticleParagraph.ErrorMessage);
            }
            return RedirectToAction("ProfilePage", "User");
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
            else
            {
                article = _context.Articles
                    .Where(a => a.Id == id && a.IsApproved == true)
                    .Include(u => u.User)
                    .Include(u => u.ArticleParagraphs)
                    .FirstOrDefault();
            }
            if (article==null||article.Id==0)
                return NotFound("The article does not exist or has not been approved by the moderator");
            return View(article);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PublishArticle(int id)
        {
            var publishedArticle = await _articleService.PublishArticle(id);
            if (!publishedArticle.IsSuccess)
                return BadRequest(publishedArticle.ErrorMessage);
            return RedirectToAction("AdminPanel", "User");
        }
    }
}
