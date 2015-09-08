namespace News.WebApiControllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using News.Data.Contracts;
    using News.Models;

    public class MockNewsRepository
    {
        public Mock<IRepository<News>> NewsRepositoryMock { get; set; }

        public void PrepareMock()
        {
            var fakeNews = new List<News>
            {
                new News
                {
                    Id = 1,
                    Title = "Title 1",
                    Content = "Content 11111",
                    PublishDate = new DateTime(2015, 03, 03)
                },
                new News
                {
                    Id = 2,
                    Title = "Title 2",
                    Content = "Content 22222",
                    PublishDate = new DateTime(2015, 05, 05)
                },
                new News
                {
                    Id = 3,
                    Title = "Title 3",
                    Content = "Content 33333",
                    PublishDate = new DateTime(2015, 01, 01)
                },
                new News
                {
                    Id = 4,
                    Title = "Title 4",
                    Content = "Content 44444",
                    PublishDate = new DateTime(2015, 04, 04)
                },
                new News
                {
                    Id = 5,
                    Title = "Title 5",
                    Content = "Content 55555",
                    PublishDate = new DateTime(2015, 02, 02)
                },
            };

            this.NewsRepositoryMock = new Mock<IRepository<News>>();
            this.NewsRepositoryMock.Setup(r => r.All())
                .Returns(fakeNews.AsQueryable());
            this.NewsRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeNews.FirstOrDefault(n => n.Id == id);
                });
            this.NewsRepositoryMock.Setup(r => r.Add(It.IsAny<News>()))
                .Returns((News news) =>
                {
                    fakeNews.Add(news);
                    return news;
                });
            this.NewsRepositoryMock.Setup(r => r.Update(It.IsAny<News>()))
                .Callback((News news) =>
                {
                    var newsToRemove = fakeNews.FirstOrDefault(n => n.Id == news.Id);
                    fakeNews.Remove(newsToRemove);
                    fakeNews.Add(news);
                });
            this.NewsRepositoryMock.Setup(r => r.Delete(It.IsAny<News>()))
                .Callback((News news) =>
                {
                    fakeNews.Remove(news);
                });
        }
    }
}
