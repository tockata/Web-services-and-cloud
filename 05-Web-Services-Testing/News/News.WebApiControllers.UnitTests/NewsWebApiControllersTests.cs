namespace News.WebApiControllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using News.Data.Contracts;
    using News.Models;
    using News.Services.Controllers;
    using News.Services.Models;

    [TestClass]
    public class NewsWebApiControllersTests
    {
        private MockNewsRepository mock;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockNewsRepository();
            this.mock.PrepareMock();
        }

        [TestMethod]
        public void List_All_News_Should_Return_200OK_And_Return_News_Correctly()
        {
            // Arrange
            var fakeNews = this.mock.NewsRepositoryMock.Object.All();
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News.All())
                .Returns(fakeNews);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.GetNews()
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var newsResponse = response.Content
                .ReadAsAsync<IEnumerable<News>>()
                .Result
                .ToList();

            var orderedFakeNews = fakeNews
                .OrderBy(n => n.PublishDate)
                .ToList();

            CollectionAssert.AreEqual(orderedFakeNews, newsResponse);
        }

        [TestMethod]
        public void Create_News_With_Correct_Data_Should_Return_201Created_Add_News_In_Repo_And_Return_News()
        {
            // Arrange
            var newNews = new NewsBindingModel
            {
                Title = "Title 10",
                Content = "Content 1010101010",
                PublishDate = new DateTime(2015, 9, 1)
            };

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.PostNews(newNews)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNews = this.mock.NewsRepositoryMock.Object.All().ToList();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(6, fakeNews.Count);
            Assert.AreEqual(newNews.Title, fakeNews[5].Title);
            Assert.AreEqual(newNews.Content, fakeNews[5].Content);
            Assert.AreEqual(newNews.PublishDate, fakeNews[5].PublishDate);
        }

        [TestMethod]
        public void Create_News_With_InCorrect_Data_Should_Return_400BadRequest()
        {
            // Arrange
            var newNews = new NewsBindingModel
            {
                Title = "T", // invalid MinLength
                Content = "Content 1010101010",
                PublishDate = new DateTime(2015, 9, 1)
            };

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);
            newsController.ModelState.AddModelError("Title", "Title MinLength is 5.");

            // Act
            var response = newsController.PostNews(newNews)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNews = this.mock.NewsRepositoryMock.Object.All().ToList();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
            Assert.AreEqual(5, fakeNews.Count);
        }

        [TestMethod]
        public void Modify_Existing_News_With_Correct_Data_Should_Return_200OK_And_Modify_News_In_Repository()
        {
            // Arrange
            var fakeNewsToModify = this.mock.NewsRepositoryMock.Object.All().FirstOrDefault();

            var newNews = new NewsBindingModel
            {
                Title = "Modified Title",
                Content = "Modified content",
                PublishDate = new DateTime(2013, 12, 12)
            };
            
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.ChangeNews(fakeNewsToModify.Id, newNews)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNews = this.mock.NewsRepositoryMock.Object.All().ToList();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(newNews.Title, fakeNews[4].Title);
            Assert.AreEqual(newNews.Content, fakeNews[4].Content);
            Assert.AreEqual(newNews.PublishDate, fakeNews[4].PublishDate);
        }

        [TestMethod]
        public void Modify_Existing_News_With_InCorrect_Data_Should_Return_400BadRequest()
        {
            // Arrange
            var fakeNewsToModify = this.mock.NewsRepositoryMock.Object.All().FirstOrDefault();

            var newNews = new NewsBindingModel
            {
                Title = "T", // invalid MinLength
                Content = "Modified content",
                PublishDate = new DateTime(2013, 12, 12)
            };

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);
            newsController.ModelState.AddModelError("Title", "Title MinLength is 5.");

            // Act
            var response = newsController.ChangeNews(fakeNewsToModify.Id, newNews)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNews = this.mock.NewsRepositoryMock.Object.All().ToList();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
            Assert.AreEqual(fakeNewsToModify.Title, fakeNews[0].Title);
            Assert.AreEqual(fakeNewsToModify.Content, fakeNews[0].Content);
            Assert.AreEqual(fakeNewsToModify.PublishDate, fakeNews[0].PublishDate);
        }

        [TestMethod]
        public void Modify_NonExisting_News_Should_Return_400BadRequest()
        {
            // Arrange
            int id = this.GenerateNonExistingId();

            var newNews = new NewsBindingModel
            {
                Title = "Modified title",
                Content = "Modified content",
                PublishDate = new DateTime(2013, 12, 12)
            };

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.ChangeNews(id, newNews)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void Delete_Existing_News_Should_Return_200OK_And_Delete_News_From_Repository()
        {
            // Arrange
            var fakeNewsBeforeDeleting = this.mock.NewsRepositoryMock.Object.All().ToList();

            var fakeNewsToDelte = fakeNewsBeforeDeleting[0];

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.DeleteNews(fakeNewsToDelte.Id)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNewsAfterDeleting = this.mock.NewsRepositoryMock.Object.All().ToList();
            var deletedFakeNewsInRepo = fakeNewsAfterDeleting
                .FirstOrDefault(n => n.Id == fakeNewsToDelte.Id);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsNull(deletedFakeNewsInRepo);
            Assert.AreEqual(4, fakeNewsAfterDeleting.Count);
        }

        [TestMethod]
        public void Delete_NonExisting_News_Should_Return_400BadRequest()
        {
            // Arrange
            int id = this.GenerateNonExistingId();

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(c => c.News)
                .Returns(this.mock.NewsRepositoryMock.Object);

            var newsController = new NewsController(mockContext.Object);
            this.SetupController(newsController);

            // Act
            var response = newsController.DeleteNews(id)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeNewsAfterDeleting = this.mock.NewsRepositoryMock.Object.All().ToList();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
            Assert.AreEqual(5, fakeNewsAfterDeleting.Count);
        }

        private void SetupController(NewsController newsController)
        {
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();
        }

        private int GenerateNonExistingId()
        {
            Random rnd = new Random();
            while (true)
            {
                int id = rnd.Next(1, int.MaxValue);
                var fakeNewsInMockRepo = this.mock.NewsRepositoryMock.Object.All()
                    .FirstOrDefault(n => n.Id == id);

                if (fakeNewsInMockRepo == null)
                {
                    return id;
                }
            }
        }
    }
}
