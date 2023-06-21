using Memochka.Models.Entities;

namespace Memochka.Services.Interfaces
{
    public interface IMeme
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateMemeAsync(string userLogin, Meme meme, IFormFileCollection files);
        Task<(bool IsSuccess, string ErrorMessage)> UpMemeViewsAsync(int memeId);
        Task<(bool IsSuccess, string ErrorMessage, List<Meme> MemesList)> GetOrderedemesAsync(string category, int year);
        Task<(bool IsSuccess, string ErrorMessage)> PublishMeme(int id);
    }
}
