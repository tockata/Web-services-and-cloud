namespace OnlineShop.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OnlineShop.Data;
    using OnlineShop.Models;
    using OnlineShop.Services;

    using Owin;

    [TestClass]
    public class AdsIntegrationTests
    {
        private const string TestUserUsername = "username";
        private const string TestUserPassword = "paSs123#word";
        private static TestServer httpTestServer;
        private static HttpClient httpClient;
        private string accessToken;

        public string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginRespone = this.Login();
                    if (!loginRespone.IsSuccessStatusCode)
                    {
                        Assert.Fail("Unable to login: " + loginRespone.ReasonPhrase);
                    }

                    var loginData = loginRespone.Content.ReadAsAsync<LoginDto>().Result;
                    this.accessToken = loginData.Access_Token;
                }

                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                var startUp = new Startup();

                startUp.Configuration(appBuilder);
                appBuilder.UseWebApi(config);
            });

            httpClient = httpTestServer.HttpClient;

            SeedDatabase();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }

            CleanDatabase();
        }

        [TestMethod]
        public void Login_Should_Return_200OK_And_Access_Token()
        {
            var loginResponse = this.Login();
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginData = loginResponse.Content.ReadAsAsync<LoginDto>().Result;
            Assert.IsNotNull(loginData.Access_Token);
        }

        [TestMethod]
        public void Posting_Ad_With_Invalid_AdType_Should_Return_Bad_Request()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"), 
                new KeyValuePair<string, string>("description", "some description"),
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", "-1"), // invalid Id
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString())
            });

            var response = this.PostNewAd(data);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Posting_Ad_Without_Categories_Should_Return_Bad_Request()
        {
            var context = new OnlineShopContext();
            var adType = context.AdTypes.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"), 
                new KeyValuePair<string, string>("description", "some description"),
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString())
            });

            var response = this.PostNewAd(data);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Posting_Ad_With_More_Than_3_Categories_Should_Return_Bad_Request()
        {
            var context = new OnlineShopContext();
            var categories = context.Categories.ToList();
            var adType = context.AdTypes.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Atra"),
                new KeyValuePair<string, string>("description", "some description"),
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", categories[0].Id.ToString()),
                new KeyValuePair<string, string>("categories[1]", categories[1].Id.ToString()),
                new KeyValuePair<string, string>("categories[2]", categories[2].Id.ToString()),
                new KeyValuePair<string, string>("categories[3]", categories[3].Id.ToString()),
            });

            var response = this.PostNewAd(data);
            var ads = context.Ads.ToList();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Posting_Ad_Without_Name_Should_Return_Bad_Request()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.First();
            var adType = context.AdTypes.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("description", "some description"),
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString())
            });

            var response = this.PostNewAd(data);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Posting_Valid_Ad_Should_Return_200OK_And_Create_Ad()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.First();
            var adType = context.AdTypes.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"),
                new KeyValuePair<string, string>("description", "some description"),
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString())
            });

            var response = this.PostNewAd(data);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var ads = context.Ads.ToList();
            var newAdd = ads[ads.Count - 1];
            Assert.AreEqual(3, ads.Count);
            Assert.AreEqual("Opel Astra", newAdd.Name);
            Assert.AreEqual("some description", newAdd.Description);
            Assert.AreEqual(2000, newAdd.Price);
        }

        private static void SeedDatabase()
        {
            var context = new OnlineShopContext();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser
            {
                UserName = TestUserUsername,
                Email = "testUser@email.com"
            };

            var result = userManager.CreateAsync(user, TestUserPassword).Result;
            if (!result.Succeeded)
            {
                Assert.Fail(string.Join(Environment.NewLine, result.Errors));
            }

            SeedCategories(context);
            SeedAdTypes(context);
        }

        private static void SeedCategories(OnlineShopContext context)
        {
            context.Categories.Add(new Category { Name = "Cars" });
            context.Categories.Add(new Category { Name = "Phones" });
            context.Categories.Add(new Category { Name = "Cameras" });
            context.SaveChanges();
        }

        private static void SeedAdTypes(OnlineShopContext context)
        {
            context.AdTypes.Add(new AdType { Name = "Normal", Index = 100 });
            context.AdTypes.Add(new AdType { Name = "Premium", Index = 200 });
            context.AdTypes.Add(new AdType { Name = "Gold", Index = 300 });
            context.SaveChanges();
        }

        private static void CleanDatabase()
        {
            var context = new OnlineShopContext();
            context.Ads.Delete();
            context.AdTypes.Delete();
            context.Categories.Delete();
            context.Users.Delete();
            context.SaveChanges();
        }

        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", TestUserUsername), 
                new KeyValuePair<string, string>("password", TestUserPassword), 
                new KeyValuePair<string, string>("grant_type", "password"), 
            });

            var response = httpClient.PostAsync("/Token", loginData).Result;
            return response;
        }

        private HttpResponseMessage PostNewAd(FormUrlEncodedContent data)
        {
            if (httpClient.DefaultRequestHeaders.Authorization == null)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);
            }
            
            return httpClient.PostAsync("/api/ads", data).Result;
        }
    }
}
