using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Memochka.Services
{
    public class ArticleService : IArticle
    {
        private readonly MemochkaContext _context;
        public ArticleService(MemochkaContext context) => _context = context;
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateArticleAsync(Article article, string userLogin, IFormFile file)
        {
            (bool,string) savedDataArticleResult = (true,string.Empty);
            if (article == null)
                return (false, "Article is empty");
            if (article.Title.IsNullOrEmpty())
                return (false, "Title is empty");
            if (!_context.Articles.Any(a => a.Title == article.Title))
            {
                int userId = await _context.Users
                    .Where(u => u.Login == userLogin)
                    .Select(u => u.Id).FirstOrDefaultAsync();
                _context.Articles.Add(article with
                {
                    Views = 0,
                    PublicationDate = DateTime.Now,
                    UserId = userId,
                });
                var savedDataArticle = await _context.SaveChangesAsync();
                savedDataArticleResult = savedDataArticle == 0 ? (false, "Something went wrong when adding Article to db") : (true, string.Empty);
                int articleId = await _context.Articles
                    .Where(a=>a.Title==article.Title)
                    .Select(a=>a.Id).FirstOrDefaultAsync();
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "articles",
                    $"{articleId}MainImg.jpg"
                );
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            var emptyArticleParagraph = new ArticleParagraph
            {
                Title = string.Empty,
                Description = string.Empty,
                ArticleId = await _context.Articles.Where(a => a.Title == article.Title).Select(a => a.Id)
                    .FirstOrDefaultAsync(),
                ImageId = 0
            };
            await _context.ArticleParagraphs.AddAsync(emptyArticleParagraph);
            var savedData = await _context.SaveChangesAsync();
            var savedDataArticleParResult = savedData == 0 ? (false, "Something went wrong when adding ArticleParagraphs to db") : (true, string.Empty);
            if (!savedDataArticleResult.Item1)
                return (false, savedDataArticleResult.Item2);
            if (!savedDataArticleParResult.Item1)
                return (false, savedDataArticleParResult.Item2);
            return (true,string.Empty);
        }
    }
}
