using Memochka.Models.Entities;

namespace Memochka.Services.Interfaces
{
    public interface IMeme
    {
        Task<(bool IsSuccess, string ErrorMessage)> CreateMemeAsync(string userLogin, Meme meme, IFormFileCollection files);
    }
}
