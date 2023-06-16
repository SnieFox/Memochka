using Memochka.Models.Entities;

namespace Memochka.Services.Interfaces
{
    public interface IArticle
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateArticleAsync(Article article, string userLogin, IFormFile file);
    }
}
