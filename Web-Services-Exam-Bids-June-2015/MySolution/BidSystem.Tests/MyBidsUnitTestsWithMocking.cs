namespace BidSystem.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using BidSystem.Data.Contracts;
    using BidSystem.RestServices.Controllers;
    using BidSystem.RestServices.Infrastructure;
    using BidSystem.RestServices.Models.Bids;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class MyBidsUnitTestsWithMocking
    {
        private MockContainer mocks;

        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }

        [TestMethod]
        public void List_User_Bids_Of_Existing_And_Logged_User_Should_List_Bids()
        {
            // Arrange
            var fakeBids = this.mocks.BidRepositoryMock.Object.All();
            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();
            var fakeUser = this.mocks.UserRepositoryMock.Object.All()
                .FirstOrDefault();

            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available.");
            }

            var mockContext = new Mock<IBidSystemData>();
            mockContext.Setup(c => c.Bids.All())
                .Returns(fakeBids);
            mockContext.Setup(c => c.Users.All())
                .Returns(fakeUsers);
            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(uip => uip.GetUserId())
                .Returns(fakeUser.Id);

            var bidsController = new BidsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(bidsController);

            // Act
            var response = bidsController.ListUserBids()
                .ExecuteAsync(CancellationToken.None).Result;
            var results = response.Content.ReadAsAsync<List<BidViewModel>>().Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("gosho", results[0].Bidder);
            Assert.AreEqual("gosho", results[1].Bidder);
            Assert.AreEqual(99, results[0].Id);
            Assert.AreEqual(55, results[1].Id);
        }

        [TestMethod]
        public void List_User_Bids_Of_NonExisting_User_Should_Return_401Unauthorized()
        {
            // Arrange
            var fakeBids = this.mocks.BidRepositoryMock.Object.All();
            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();
            
            var mockContext = new Mock<IBidSystemData>();
            mockContext.Setup(c => c.Bids.All())
                .Returns(fakeBids);
            mockContext.Setup(c => c.Users.All())
                .Returns(fakeUsers);
            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(uip => uip.GetUserId())
                .Returns(int.MaxValue.ToString);

            var bidsController = new BidsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(bidsController);

            // Act
            var response = bidsController.ListUserBids()
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void List_User_Bids_Of_Not_Logged_User_Should_Return_401Unauthorized()
        {
            // Arrange
            var fakeBids = this.mocks.BidRepositoryMock.Object.All();
            var fakeUsers = this.mocks.UserRepositoryMock.Object.All();

            var mockContext = new Mock<IBidSystemData>();
            mockContext.Setup(c => c.Bids.All())
                .Returns(fakeBids);
            mockContext.Setup(c => c.Users.All())
                .Returns(fakeUsers);
            var mockUserIdProvider = new Mock<IUserIdProvider>();

            var bidsController = new BidsController(mockContext.Object, mockUserIdProvider.Object);
            this.SetupController(bidsController);

            // Act
            var response = bidsController.ListUserBids()
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private void SetupController(BidsController bidsController)
        {
            bidsController.Request = new HttpRequestMessage();
            bidsController.Configuration = new HttpConfiguration();
        }
    }
}
