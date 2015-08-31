namespace OnlineShop.Tests.UnitTests
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

    using OnlineShop.Data.Contracts;
    using OnlineShop.Models;
    using OnlineShop.Services.Controllers;
    using OnlineShop.Services.Infrastructure;
    using OnlineShop.Services.Models;
    
    [TestClass]
    public class AdsControllerTests
    {
        private MockContainer mocks;

        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }

        [TestMethod]
        public void GetAllAds_Should_Return_Total_Ads_Sorted_By_TypeIndex()
        {
            // Arrange
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();

            var mockContext = new Mock<IOnlineShopData>();
            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockContext.Setup(c => c.Ads.All())
                .Returns(fakeAds);

            var adsController = new AdsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(adsController);

            // Act
            var response = adsController.GetAds()
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var adsResponse = response.Content
                .ReadAsAsync<IEnumerable<AdViewModel>>()
                .Result.Select(a => a.Id)
                .ToList();

            var orderedFakeAds = fakeAds
                .OrderByDescending(a => a.Type.Index)
                .ThenBy(a => a.PostedOn)
                .Select(a => a.Id)
                .ToList();

            CollectionAssert.AreEqual(orderedFakeAds, adsResponse);
        }

        [TestMethod]
        public void CreateAd_Should_Succsessfully_Add_To_Repository()
        {
            // Arrange
            var ads = new List<Ad>();
            var fakeUser = this.mocks.UserRepositoryMock.Object.All()
                .FirstOrDefault();

            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            this.mocks.AdRepositoryMock
                .Setup(r => r.Add(It.IsAny<Ad>()))
                .Callback((Ad ad) =>
                {
                    ad.Owner = fakeUser;
                    ads.Add(ad);
                });

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(this.mocks.AdRepositoryMock.Object);
            mockContext.Setup(c => c.Categories)
                .Returns(this.mocks.CategoryRepositoryMock.Object);
            mockContext.Setup(c => c.Users)
                .Returns(this.mocks.UserRepositoryMock.Object);
            mockContext.Setup(c => c.AdTypes)
                .Returns(this.mocks.AdTypeRepositoryMock.Object);

            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(uip => uip.GetUserId())
                .Returns(fakeUser.Id);

            var adsController = new AdsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(adsController);

            var randomName = Guid.NewGuid().ToString();
            var newAd = new CreateAdBindingModel
            {
                Name = randomName,
                Price = 555,
                TypeId = 1,
                Description = "Some description.",
                Categories = new[] { 3, 1, 99 }
            };

            var response = adsController.CreateAd(newAd)
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(1, ads.Count);
            Assert.AreEqual(newAd.Name, ads[0].Name);
        }

        [TestMethod]
        public void Closing_Ad_As_Owner_Should_Return_200OK()
        {
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();
            var openAd = fakeAds.FirstOrDefault(ad => ad.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("Cannot perform test - no open ads available.");
            }

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(this.mocks.AdRepositoryMock.Object);

            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(uip => uip.GetUserId())
                .Returns(openAd.OwnerId);

            var adsController = new AdsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(adsController);

            var response = adsController.CloseAd(openAd.Id)
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsNotNull(openAd.ClosedOn);
            Assert.AreEqual(AdStatus.Closed, openAd.Status);
        }

        [TestMethod]
        public void Closing_Ad_As_NonOwner_Should_Return_400BadRequest()
        {
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();
            var openAd = fakeAds.FirstOrDefault(ad => ad.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("Cannot perform test - no open ads available.");
            }

            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();
            ApplicationUser foreignUser = fakeUsers
                .FirstOrDefault(u => u.Id != openAd.OwnerId);

            if (foreignUser == null)
            {
                Assert.Fail("Cannot perform test - no user who is non owner of the ad.");
            }

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(this.mocks.AdRepositoryMock.Object);

            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(uip => uip.GetUserId())
                .Returns(foreignUser.Id);

            var adsController = new AdsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(adsController);

            var response = adsController.CloseAd(openAd.Id)
                .ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
            Assert.IsNull(openAd.ClosedOn);
            Assert.AreEqual(AdStatus.Open, openAd.Status);
        }

        private void SetupController(AdsController adsController)
        {
            adsController.Request = new HttpRequestMessage();
            adsController.Configuration = new HttpConfiguration();
        }
    }
}
