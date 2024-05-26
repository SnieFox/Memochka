using Memochka.Services;
using Memochka.Test.Common;
using Memochka.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Memochka.Controllers;
using Memochka.Services.Interfaces;
using Moq;
using Memochka.Models.MemochkaDbContext;

namespace Memochka.Test.Services.Article
{
    public class CreateArticleServiceTests : TestServicesBase
    {
        private readonly IArticle _articleService;
        public CreateArticleServiceTests() 
        {
            _articleService = new ArticleService(_context);
        }

        [Fact]
        public async Task CreateArticleService_Success()
        {
            //Arrange
            var article = new Models.Entities.Article
            {
                Title = "Test",
                ArticleParagraphs = new List<ArticleParagraph>
                {
                    new ArticleParagraph { ParagraphTitle = "Paragraph 1", Description = "Description 1" },
                    new ArticleParagraph { ParagraphTitle = "Paragraph 2", Description = "Description 2" }
                }
            };
            string userLogin = "john";

            var formFiles = new FormFileCollection
            {
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "mainImg", "mainImg.jpg"),
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "0", "0.jpg"),
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "1", "1.jpg")
            };

            var currentDirectory = Directory.GetCurrentDirectory();
            var imagesPath = Path.Combine(currentDirectory, "wwwroot", "images", "articles");

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            //Act
            var serviceResponse = await _articleService.CreateArticleAsync(article, userLogin, formFiles);

            //Assert
            var expected = (true, string.Empty);

            Assert.Equal(expected, serviceResponse);
        }

        [Fact]
        public async Task CreateArticleService_TitleWasEmpty()
        {
            //Arrange
            var article = new Models.Entities.Article
            {
                Title = string.Empty
            };

            string userLogin = string.Empty;

            var formFiles = new FormFileCollection();

            //Act
            var serviceResponse = await _articleService.CreateArticleAsync(article, userLogin, formFiles);

            //Assert

            var expected = (false, "Title was null");

            Assert.Equal(expected, serviceResponse);
        }

        [Fact]
        public async Task CreateArticleService_ParagraphTitleWasNullOrEmpty()
        {
            //Arrange
            string userLogin = string.Empty;
            var formFiles = new FormFileCollection();
            var article = new Models.Entities.Article
            {
                Title = "Test",
                ArticleParagraphs = new List<ArticleParagraph>
                {
                    new ArticleParagraph { ParagraphTitle = "", Description = "Description 1" },
                    new ArticleParagraph { ParagraphTitle = "Paragraph 2", Description = "Description 2" }
                }
            };

            //Act
            var serviceResponse = await _articleService.CreateArticleAsync(article, userLogin, formFiles);

            //Assert

            var expected = (false, "All paragraphs must be filled out");

            Assert.Equal(expected, serviceResponse);
        }

        [Fact]
        public async Task CreateArticleService_InvalidFileName()
        {
            //Arrange
            string userLogin = string.Empty;
            var formFiles = new FormFileCollection 
            {
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "invalide", "invalide"),
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "invalide", "invalide"),
                new FormFile(new MemoryStream(new byte[0]), 0, 0, "invalide", "invalide")
            };
            var article = new Models.Entities.Article
            {
                Title = "Test",
                ArticleParagraphs = new List<ArticleParagraph>
                {
                    new ArticleParagraph { ParagraphTitle = "Paragraph 1", Description = "Description 1" },
                    new ArticleParagraph { ParagraphTitle = "Paragraph 2", Description = "Description 2" }
                }
            };

            //Act
            var serviceResponse = await _articleService.CreateArticleAsync(article, userLogin, formFiles);

            //Assert

            var expected = (false, "Invalid file name");

            Assert.Equal(expected, serviceResponse);
        }

    }
}
