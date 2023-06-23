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

        public async Task<(bool IsSuccess, string ErrorMessage, List<Meme> MemesList)> GetOrderedemesAsync(string category, int year)
        {
            try
            {
                List<Meme> memes = new List<Meme>();
                if (category == null && year == 0)
                {
                    memes = await _context.Memes
                        .Where(m=>m.IsApproved == true)
                        .OrderByDescending(m => m.PublicationDate)
                        .ToListAsync();
                }

                if (category != null)
                {
                    memes = await _context.Memes
                        .Include(m=>m.MemeCategory)
                        .Where(m => m.MemeCategory.Category==category && m.IsApproved == true)
                        .OrderByDescending(m => m.PublicationDate)
                        .ToListAsync();
                }

                if (year != 0)
                {
                    memes = await _context.Memes
                        .Where(m => m.Year == year && m.IsApproved == true)
                        .OrderByDescending(m => m.PublicationDate)
                        .ToListAsync();
                }

                if (category != null && year != 0)
                {
                    memes = await _context.Memes
                        .Include(m => m.MemeCategory.Category)
                        .Where(m => m.MemeCategory.Category == category&&m.Year == year && m.IsApproved == true)
                        .OrderByDescending(m => m.PublicationDate)
                        .ToListAsync();
                }

                return (true, string.Empty, memes);
            }
            catch (Exception e)
            {
                return (false, e.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> UpMemeViewsAsync(int memeId)
        {
            try
            {
                var meme = _context.Memes
                    .Where(m=>m.Id==memeId)
                    .FirstOrDefault();
                meme.Views++;
                _context.Memes.Update(meme);
                int savedData = await _context.SaveChangesAsync();
                return savedData==0?(false, "Something went wrong when change meme views in db"):(true,string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> CreateMemeAsync(string userLogin, Meme meme, IFormFileCollection files)
        {
            try
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
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> PublishMeme(int id)
        {
            try
            {
                var meme = await _context.Memes.FirstOrDefaultAsync(m => m.Id == id);
                if (meme==null)
                    return (false, "Meme does not exist");
                meme.IsApproved = true;
                _context.Memes.Update(meme);
                int saved = await _context.SaveChangesAsync();
                return saved==0?(false,"Something went wrong when changing data in db"):(true, string.Empty);
            }
            catch (Exception e)
            {
                return (false,e.Message);
            }
        }
    }
}
