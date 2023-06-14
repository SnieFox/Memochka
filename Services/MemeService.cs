using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Memochka.Services
{
    public class MemeService : IMeme
    {
        private MemochkaContext _context;
        public MemeService(MemochkaContext context)=>_context=context;
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateMemeAsync(string userLogin, Meme meme, IFormFileCollection files)
        {
            if (meme == null)
                return (false, "User data is null");
            if (files.Count == 0)
                return (false, "Images data is null");
            int userId = await _context.Users
                .Where(u => u.Login == userLogin)
                .Select(u => u.Id).FirstOrDefaultAsync();
            await _context.Memes.AddAsync(meme with
            {
                PublicationDate = DateTime.Now,
                IsApproved = false,
                Views = 0,
                UserId = userId,
            });
            int savedMeme = await _context.SaveChangesAsync();
            var memeResult = savedMeme == 0 ? (false, "Something went wrong when adding meme to db") : (true, string.Empty);
            meme.Id = _context.Memes
                .Max(m => m.Id);
            int tempMemeId = 1;
            var path = string.Empty;
            foreach (var file in files)
            {
                switch (file.Name)
                {
                    case "mainImg":
                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "memes",
                            $"{meme.Id}MainImg.jpg"
                        );
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        break;
                    case "originImg":
                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "memes",
                            $"{meme.Id}OriginImg.jpg"
                        );
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        break;
                    case "memeImg":
                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "memes",
                            $"{meme.Id}Meme-{tempMemeId}.jpg"
                        );
                        await using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        await _context.MemePictures.AddAsync(new MemePicture
                        {
                            MemeId = meme.Id,
                            PictureId = $"{meme.Id}Meme-{tempMemeId}.jpg"
                        });
                        tempMemeId++;
                        break;
                    default:
                        return (false, "Invalid picture name");
                }
            }

            int savedPicture = await _context.SaveChangesAsync();
            var picturesResult = savedPicture == 0 ? (false, "Something went wrong when adding pictures to db") : (true, string.Empty);

            if (!memeResult.Item1)
                return (false, memeResult.Item2);
            if (!picturesResult.Item1)
                return (false, picturesResult.Item2);
            return (true, string.Empty);
        }
    }
}
