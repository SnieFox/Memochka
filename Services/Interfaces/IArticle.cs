using Memochka.Models.Entities;

namespace Memochka.Services.Interfaces
{
    public interface IArticle
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateArticleAsync(Article article, string userLogin, IFormFileCollection files);
        Task<(bool IsSuccess, string ErrorMessage)> UpArticleViewsAsync(int articleId);
        Task<(bool IsSuccess, string ErrorMessage)> PublishArticle(int id);
    }
}
