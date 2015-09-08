namespace News.Repositories.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Transactions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using News.Data;
    using News.Models;

    [TestClass]
    public class NewsRepositoriesTests
    {
        private static TransactionScope tran;

        private EfRepository<News> repo;
            
        [TestInitialize]
        public void TestInit()
        {
            tran = new TransactionScope();
            this.repo = new EfRepository<News>(new NewsContext());
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            tran.Dispose();
        }

        [TestMethod]
        public void Get_All_News_Should_Return_News_Correctly()
        {
            // Arrange
            var news1 = new News
            {
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            var news2 = new News
            {
                Title = "Title2",
                Content = "Content 222",
                PublishDate = new DateTime(2015, 02, 02)
            };

            var news3 = new News
            {
                Title = "Title3",
                Content = "Content 333",
                PublishDate = new DateTime(2015, 03, 03)
            };
            
            List<News> newsList = new List<News> { news1, news2, news3 };

            this.repo.Add(news1);
            this.repo.Add(news2);
            this.repo.Add(news3);
            this.repo.SaveChanges();

            // Act
            var newsInDb = this.repo.All().ToList();

            // Assert
            CollectionAssert.AreEqual(newsList, newsInDb);
        }

        [TestMethod]
        public void Create_News_With_Correct_Data_Creates_And_Return_News()
        {
            // Arrange
            var news = new News
            {
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };   

            // Act
            var newsInDb = this.repo.Add(news);
            this.repo.SaveChanges();

            // Assert
            Assert.AreEqual(news.Title, newsInDb.Title);
            Assert.AreEqual(news.Content, newsInDb.Content);
            Assert.AreEqual(news.PublishDate, newsInDb.PublishDate);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Create_News_With_InCorrect_Data_Should_Throw_Exception()
        {
            // Arrange
            var news = new News
            {
                Title = "T1", // invalid MinLength
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            // Act
            var newsInDb = this.repo.Add(news);
            this.repo.SaveChanges();
        }

        [TestMethod]
        public void Modify_Existing_News_With_Correct_Data_Should_Modify_News()
        {
            // Arrange
            var news = new News
            {
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            var newsInDb = this.repo.Add(news);
            this.repo.SaveChanges();

            // Act
            newsInDb.Title = "NewTitle";
            this.repo.Update(newsInDb);
            this.repo.SaveChanges();
            var modifiedNews = this.repo.All().FirstOrDefault();

            // Assert
            Assert.AreEqual(newsInDb.Title, modifiedNews.Title);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Modify_Existing_News_With_Incorrect_Data_Should_Throw_Exception()
        {
            // Arrange
            var news = new News
            {
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            var newsInDb = this.repo.Add(news);
            this.repo.SaveChanges();

            // Act
            newsInDb.Title = "T1"; // invalid MinLength
            this.repo.Update(newsInDb);
            this.repo.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void Modify_NonExisting_News_Should_Throw_Exception()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            // Act
            news.Title = "Title2";
            this.repo.Update(news);
            this.repo.SaveChanges();
        }

        [TestMethod]
        public void Delete_Existing_News_Should_Delete_News()
        {
            // Arrange
            var news = new News
            {
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            var newsInDb = this.repo.Add(news);
            this.repo.SaveChanges();

            // Act
            this.repo.Delete(newsInDb);
            this.repo.SaveChanges();

            // Assert
            Assert.AreEqual(0, this.repo.All().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void Delete_NonExisting_News_Should_Throw_Exception()
        {
            // Arrange
            var news = new News
            {
                Id = 1,
                Title = "Title1",
                Content = "Content 111",
                PublishDate = new DateTime(2015, 01, 01)
            };

            // Act
            this.repo.Delete(news);
            this.repo.SaveChanges();
        }
    }
}