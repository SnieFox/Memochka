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
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateArticleAsync(Article article, string userLogin, IFormFileCollection files)
        {
            if (article.Title.IsNullOrEmpty())
                return (false, "Title was null");

            var userId = await _context.Users
                .Where(u => u.Login == userLogin)
                .Select(u => u.Id).FirstOrDefaultAsync();
            var articleDb = new Article
            {
                Title = article.Title,
                Views = 0,
                PublicationDate = DateTime.Now,
                UserId = userId
            };
            await _context.Articles.AddAsync(articleDb);
            var savedDataArticle = await _context.SaveChangesAsync();
            var savedDataArticleResult = savedDataArticle == 0 ? (false, "Something went wrong when adding Article to db") : (true, string.Empty);
            if (!savedDataArticleResult.Item1)
                return (false, savedDataArticleResult.Item2);

            int articleId = await _context.Articles.MaxAsync(a => a.Id);
            (bool IsSuccess,string ErrorMessage) savedDataArticleParagraphResult;
            int imageIndex = 0;
            foreach (var paragraph in article.ArticleParagraphs)
            {
                if (paragraph.ParagraphTitle.IsNullOrEmpty())
                {
                    _context.Articles.Remove(await _context.Articles
                        .Where(a=>a.Id==articleId).FirstOrDefaultAsync());
                    await _context.SaveChangesAsync();

                    return (false, "All paragraphs must be filled out");
                }
                await _context.ArticleParagraphs.AddAsync(paragraph with
                {
                    ArticleId = articleId
                });
                var savedDataArticleParagraph = await _context.SaveChangesAsync();
                savedDataArticleParagraphResult = savedDataArticleParagraph == 0 ? (false, "Something went wrong when adding Article Paragraph to db") : (true, string.Empty);
                if (!savedDataArticleParagraphResult.IsSuccess)
                    return (false, savedDataArticleParagraphResult.ErrorMessage);
            }

            foreach (var file in files)
            {
                if (file.Name == imageIndex.ToString())
                {
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "articles",
                        $"{articleId}Paragraph-{imageIndex}.jpg"
                    );
                    await using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imageIndex++;
                }
                else if (file.Name == "mainImg")
                {
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
                else
                    return (false, "Invalid file name");
            }
            return (true,string.Empty);
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> UpArticleViewsAsync(int articleId)
        {
            var article = _context.Articles
                .Where(a => a.Id == articleId)
                .FirstOrDefault();
            article.Views++;
            _context.Articles.Update(article);
            int savedData = await _context.SaveChangesAsync();
            return savedData == 0 ? (false, "Something went wrong when change article views in db") : (true, string.Empty);
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> PublishArticle(int id)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
                return (false, "Article does not exist");
            article.IsApproved = true;
            _context.Articles.Update(article);
            int saved = await _context.SaveChangesAsync();
            return saved == 0 ? (false, "Something went wrong when changing data in db") : (true, string.Empty);
        }
    }
}
