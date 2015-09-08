namespace NewsServices.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using News.Data;
    using News.Models;
    using News.Services;

    using Owin;

    [TestClass]
    public class NewsServicesIntegrationTests
    {
        private const string Endpoint = "http://localhost/api/news";
        private static TestServer httpTestServer;
        private static HttpClient httpClient;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                appBuilder.UseWebApi(config);
            });

            httpClient = httpTestServer.HttpClient;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            SeedDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            CleanDatabase();
        }

        [TestMethod]
        public void List_All_News_Should_Return_200OK_And_Return_All_News_As_Json()
        {
            var context = new NewsContext();
            var newsInDb = context.News.Select(n => n.Id).ToList();

            var response = httpClient.GetAsync(Endpoint).Result;
            var newsJson = response.Content.ReadAsAsync<List<News>>().Result
                .Select(n => n.Id).ToList();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(newsInDb, newsJson);
        }

        [TestMethod]
        public void Create_News_With_Correct_Data_Should_Return_201Created_And_Created_Item()
        {
            var context = new NewsContext();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("title", "Title 10"),
                new KeyValuePair<string, string>("content", "Content 1010101010"),
                new KeyValuePair<string, string>("publishDate", "2015-9-1"),
            });

            var response = httpClient.PostAsync(Endpoint, data).Result;
            var returnedNews = response.Content.ReadAsAsync<News>().Result;
            var lastNewsInDbId = context.News.Select(n => n.Id).ToList().Last();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(lastNewsInDbId, returnedNews.Id);
        }

        [TestMethod]
        public void Create_News_With_InCorrect_Data_Should_Return_400BadRequest()
        {
            var context = new NewsContext();
            var newsInDbBeforeAdding = context.News.ToList();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("title", "T"), // invalid title MinLength
                new KeyValuePair<string, string>("content", "Content 1010101010"),
                new KeyValuePair<string, string>("publishDate", "2015-9-1"),
            });

            var response = httpClient.PostAsync(Endpoint, data).Result;
            var newsInDbAfterAdding = context.News.ToList();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            CollectionAssert.AreEqual(newsInDbBeforeAdding, newsInDbAfterAdding);
        }

        [TestMethod]
        public void Modify_Existing_News_With_Correct_Data_Should_Return_200OK_And_Modify_News_In_Db()
        {
            var context = new NewsContext();
            var newsToModify = context.News.First();

            var endpointWithId = Endpoint + "/" + newsToModify.Id;

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("title", "Modified Title"),
                new KeyValuePair<string, string>("content", "Modified Content"),
                new KeyValuePair<string, string>("publishDate", "2015-9-3"),
            });

            var response = httpClient.PutAsync(endpointWithId, data).Result;
            var returnedNews = response.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Modified Title", returnedNews.Title);
            Assert.AreEqual("Modified Content", returnedNews.Content);
            Assert.AreEqual(new DateTime(2015, 9, 3).ToString(), returnedNews.PublishDate.ToString());
        }

        [TestMethod]
        public void Modify_Existing_News_With_InCorrect_Data_Should_Return_400BadRequest()
        {
            var context = new NewsContext();
            var newsToModify = context.News.First();

            var endpointWithId = Endpoint + "/" + newsToModify.Id;

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("title", "T"), // invalid title MinLength
                new KeyValuePair<string, string>("content", "Modified Content"),
                new KeyValuePair<string, string>("publishDate", "2015-9-3"),
            });

            var response = httpClient.PutAsync(endpointWithId, data).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Delete_Existing_News_Should_Return_200OK()
        {
            var context = new NewsContext();
            var newsInDbBeforeDeleting = context.News.ToList();
            var newsToDelete = context.News.First();

            var endpointWithId = Endpoint + "/" + newsToDelete.Id;

            var response = httpClient.DeleteAsync(endpointWithId).Result;
            var newsInDbAfterDeleting = context.News.ToList();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(newsInDbBeforeDeleting.Count - 1, newsInDbAfterDeleting.Count);
        }

        [TestMethod]
        public void Delete_NonExisting_News_Should_Return_400BadRequest()
        {
            var context = new NewsContext();
            var newsInDbBeforeDeleting = context.News.ToList();
            var nonExistingNewsId = -1;

            Random rnd = new Random();
            while (true)
            {
                int id = rnd.Next(1, int.MaxValue);
                var newsInDb = newsInDbBeforeDeleting
                    .FirstOrDefault(n => n.Id == id);

                if (newsInDb == null)
                {
                    nonExistingNewsId = id;
                    break;
                }
            }

            var endpointWithId = Endpoint + "/" + nonExistingNewsId;

            var response = httpClient.DeleteAsync(endpointWithId).Result;
            var newsInDbAfterDeleting = context.News.ToList();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(newsInDbBeforeDeleting.Count, newsInDbAfterDeleting.Count);
        }

        private static void SeedDatabase()
        {
            var context = new NewsContext();
            context.News.Add(
                new News { Title = "Title 1", Content = "Content 11111", PublishDate = new DateTime(2015, 1, 1) });
            context.News.Add(
                new News { Title = "Title 2", Content = "Content 22222", PublishDate = new DateTime(2015, 2, 2) });
            context.News.Add(
                new News { Title = "Title 3", Content = "Content 33333", PublishDate = new DateTime(2015, 3, 3) });
            context.News.Add(
                new News { Title = "Title 4", Content = "Content 44444", PublishDate = new DateTime(2015, 4, 4) });
            context.News.Add(
                new News { Title = "Title 5", Content = "Content 55555", PublishDate = new DateTime(2015, 5, 5) });

            context.SaveChanges();
        }

        private static void CleanDatabase()
        {
            var context = new NewsContext();
            context.News.Delete();
            context.SaveChanges();
        }
    }
}
