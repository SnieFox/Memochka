using Memochka.Controllers;
using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services;
using Memochka.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using Moq;

namespace Memochka.Test
{
    public class ArticleControllerTests
    {
        private readonly ArticleController _controller;
        private readonly Mock<IArticle> _articleService;
        private readonly MemochkaContext _context;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<IFormFileCollection> _formFilesMock;


        public ArticleControllerTests()
        {
            _formFilesMock = new Mock<IFormFileCollection>();
            _httpContextMock = new Mock<HttpContext>();
            _context = new MemochkaContext(new DbContextOptionsBuilder<MemochkaContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options);
            _articleService = new Mock<IArticle>();
            _controller = new ArticleController(_articleService.Object, _context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContextMock.Object
                }
            };

        }
        [Fact]
        public void CreateArticle_InvalidServiceResponse_ReturnBadRequest()
        {
            var requestModel = new Article
            {
                Title = string.Empty
            };
            // Arrange
            _httpContextMock
                .Setup(h => h.User.Identity.Name)
                .Returns("User");
            _formFilesMock
                .Setup(f => f.Count)
                .Returns(1);
            var formFiles = new FormFileCollection
            {
                new FormFile(Stream.Null, 0,0,"",""),
                new FormFile(Stream.Null, 0,0,"","")
            };
            _httpContextMock
                .SetupGet(h => h.Request.Form.Files)
                .Returns(formFiles);
            _articleService
                .Setup(s => s.CreateArticleAsync(It.IsAny<Article>(), It.IsAny<string>(), It.IsAny<IFormFileCollection>()))
                .ReturnsAsync((false, "Title was null"));

            // Act
            var result = _controller.CreateArticle(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(BadRequestObjectResult),result.GetType());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.Equal(400,badRequestObjectResult.StatusCode);
            Assert.Equal("Title was null", badRequestObjectResult.Value);

        }

        [Fact]
        public void CreateArticle_ValidServiceResponse_ReturnRedirectToAction()
        {
            var requestModel = new Article();
            // Arrange
            _httpContextMock
                .Setup(h => h.User.Identity.Name)
                .Returns("User");
            _formFilesMock
                .Setup(f => f.Count)
                .Returns(1);
            var formFiles = new FormFileCollection
            {
                new FormFile(Stream.Null, 0,0,"",""),
                new FormFile(Stream.Null, 0,0,"","")
            };
            _httpContextMock
                .SetupGet(h => h.Request.Form.Files)
                .Returns(formFiles);
            _articleService
                .Setup(s => s.CreateArticleAsync(It.IsAny<Article>(), It.IsAny<string>(), It.IsAny<IFormFileCollection>()))
                .ReturnsAsync((true, string.Empty));

            // Act
            var result = _controller.CreateArticle(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(RedirectToActionResult), result.GetType());

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("ProfilePage", redirectToActionResult.ActionName);
            Assert.Equal("User", redirectToActionResult.ControllerName);

        }

        [Fact]
        public void CreateArticle_EmptyFilesInput_ReturnRedirectToAction()
        {
            var requestModel = new Article();
            // Arrange
            _httpContextMock
                .Setup(h => h.User.Identity.Name)
                .Returns("User");
            _formFilesMock
                .Setup(f => f.Count)
                .Returns(0);
            var formFiles = new FormFileCollection();
            _httpContextMock
                .SetupGet(h => h.Request.Form.Files)
                .Returns(formFiles);

            // Act
            var result = _controller.CreateArticle(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(RedirectToActionResult), result.GetType());

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("CreateArticlePage", redirectToActionResult.ActionName);
        }
        
        [Fact]
        public void ArticlePage_InvalidServiceResponse_ReturnBadRequest()
        {

            // Arrange
            int requestModel = 5;
            _articleService
                .Setup(s => s.UpArticleViewsAsync(It.IsAny<int>()))
                .ReturnsAsync((false, "Something went wrong when change article views in db"));
            // Act
            var result = _controller.ArticlePage(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(BadRequestObjectResult), result.GetType());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.Equal("Something went wrong when change article views in db", badRequestObjectResult.Value);

        }

        [Fact]
        public void ArticlePage_NullArticleId_ReturnNotFound()
        {

            // Arrange
            int requestModel = 5;
            _articleService
                .Setup(s => s.UpArticleViewsAsync(It.IsAny<int>()))
                .ReturnsAsync((true, string.Empty));
            _httpContextMock
                .Setup(h => h.User.IsInRole("Admin"))
                .Returns(true);
            // Act
            var result = _controller.ArticlePage(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(NotFoundObjectResult), result.GetType());

            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.Equal(404, notFoundObjectResult.StatusCode);
            Assert.Equal("The article does not exist or has not been approved by the moderator", notFoundObjectResult.Value);

        }
        
        [Fact]
        public void ArticlePage_ValidServiceResponseAndUserIsAdmin_ReturnView()
        {

            // Arrange
            int requestModel = 5;
            _articleService
                .Setup(s => s.UpArticleViewsAsync(It.IsAny<int>()))
                .ReturnsAsync((true, string.Empty));
            _httpContextMock
                .Setup(h => h.User.IsInRole("Admin"))
                .Returns(true);
            var article = new Article
            {
                Id = requestModel,
                IsApproved = true,
                User = new User(),
                ArticleParagraphs = new List<ArticleParagraph>()
            };
            _context.Articles.Add(article);
            _context.SaveChanges();
            // Act
            var result = _controller.ArticlePage(requestModel).Result;

            // Arrange
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(article, viewResult.Model);

        }

        [Fact]
        public void PublishArticle_InvalidServiceResponse_ReturnBadRequest()
        {

            // Arrange
            int requestModel = 5;
            _articleService
                .Setup(s => s.PublishArticle(It.IsAny<int>()))
                .ReturnsAsync((false, "Article does not exist"));
            // Act
            var result = _controller.PublishArticle(requestModel).Result;

            // Arrange
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, viewResult.StatusCode);
            Assert.Equal("Article does not exist", viewResult.Value);

        }

        [Fact]
        public void PublishArticle_ValidServiceResponse_ReturnRedirectToAction()
        {

            // Arrange
            int requestModel = 5;
            _articleService
                .Setup(s => s.PublishArticle(It.IsAny<int>()))
                .ReturnsAsync((true, string.Empty));
            // Act
            var result = _controller.PublishArticle(requestModel).Result;

            // Arrange
            Assert.Equal(typeof(RedirectToActionResult), result.GetType());

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("AdminPanel", redirectToActionResult.ActionName);
            Assert.Equal("User", redirectToActionResult.ControllerName);

        }
    }
}