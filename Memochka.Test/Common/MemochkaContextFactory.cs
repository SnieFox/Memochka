using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memochka.Test.Common
{
    public static class MemochkaContextFactory
    {
        public static MemochkaContext Create() 
        {
            var options = new DbContextOptionsBuilder<MemochkaContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new MemochkaContext(options);

            context.Database.EnsureCreated();

            AddTestUsers(context);
            AddTestArticles(context);
            AddTestArticleParagraphs(context);
            AddTestMemes(context);
            AddTestMemeCategories(context);
            AddTestMemePictures(context);
            AddTestRoles(context);

            return context;
        }

        public static void Destroy(MemochkaContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }


        private static void AddTestUsers(MemochkaContext context)
        {
            var users = new List<User>
            {
                new User { FirstName = "John", LastName = "Doe", Nickname = "john_doe", Login = "john", Password = "password" },
                new User { FirstName = "Jane", LastName = "Doe", Nickname = "jane_doe", Login = "jane", Password = "password" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static void AddTestArticles(MemochkaContext context)
        {
            var articles = new List<Article>
            {
                new Article { Title = "Test Article 1", Views = 10, PublicationDate = DateTime.Now, UserId = 1, IsApproved = true },
                new Article { Title = "Test Article 2", Views = 15, PublicationDate = DateTime.Now, UserId = 2, IsApproved = true }
            };

            context.Articles.AddRange(articles);
            context.SaveChanges();
        }

        private static void AddTestArticleParagraphs(MemochkaContext context)
        {
            var paragraphs = new List<ArticleParagraph>
            {
                new ArticleParagraph { ParagraphTitle = "Paragraph 1", Description = "Description 1", ArticleId = 1 },
                new ArticleParagraph { ParagraphTitle = "Paragraph 2", Description = "Description 2", ArticleId = 1 },
                new ArticleParagraph { ParagraphTitle = "Paragraph 1", Description = "Description 1", ArticleId = 2 },
                new ArticleParagraph { ParagraphTitle = "Paragraph 2", Description = "Description 2", ArticleId = 2 }
            };

            context.ArticleParagraphs.AddRange(paragraphs);
            context.SaveChanges();
        }

        private static void AddTestMemes(MemochkaContext context)
        {
            var memes = new List<Meme>
            {
                new Meme { Title = "Test Meme 1", Meaning = "Meaning 1", Origins = "Origins 1", Year = 2010, PublicationDate = DateTime.Now, UserId = 1, CategoryId = 1, IsApproved = true, Views = 20 },
                new Meme { Title = "Test Meme 2", Meaning = "Meaning 2", Origins = "Origins 2", Year = 2015, PublicationDate = DateTime.Now, UserId = 2, CategoryId = 2, IsApproved = true, Views = 25 }
            };

            context.Memes.AddRange(memes);
            context.SaveChanges();
        }

        private static void AddTestMemeCategories(MemochkaContext context)
        {
            var categories = new List<MemeCategory>
            {
                new MemeCategory { Category = "Category 1" },
                new MemeCategory { Category = "Category 2" }
            };

            context.MemeCategories.AddRange(categories);
            context.SaveChanges();
        }

        private static void AddTestMemePictures(MemochkaContext context)
        {
            var pictures = new List<MemePicture>
            {
                new MemePicture { PictureId = "2Meme-1.jpg", MemeId = 2 },
                new MemePicture { PictureId = "2Meme-2.jpg", MemeId = 2 },
                new MemePicture { PictureId = "2Meme-3.jpg", MemeId = 2 },
                new MemePicture { PictureId = "2Meme-4.jpg", MemeId = 2 }
            };

            context.MemePictures.AddRange(pictures);
            context.SaveChanges();
        }

        private static void AddTestRoles(MemochkaContext context)
        {
            var roles = new List<Role>
            {
                new Role { Roles = "Admin" },
                new Role { Roles = "User" }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }
}
