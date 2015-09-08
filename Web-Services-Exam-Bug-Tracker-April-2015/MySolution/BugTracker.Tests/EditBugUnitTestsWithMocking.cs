namespace BugTracker.Tests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using BugTracker.Data.Contracts;
    using BugTracker.RestServices.Controllers;
    using BugTracker.RestServices.Models.Bugs;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class EditBugUnitTestsWithMocking
    {
        private MockContainer mock;

        [TestInitialize]
        public void InitTest()
        {
            this.mock = new MockContainer();
            this.mock.PrepareMock();
        }

        [TestMethod]
        public void Modify_Existing_Bug_With_Correct_Data_Should_Return_200OK_And_Modify_Bug_In_Repository()
        {
            // Arrange
            var fakeBugToModify = this.mock.BugRepositoryMock.Object.All().FirstOrDefault();
            var newBugData = new EditBugBindingModel
            {
                Title = "Modified title",
                Description = "Modified description",
                Status = "Closed"
            };

            var mockContext = new Mock<IBugTrackerData>();
            mockContext.Setup(c => c.Bugs)
                .Returns(this.mock.BugRepositoryMock.Object);

            var bugsController = new BugsController(mockContext.Object);
            this.SetupController(bugsController);

            // Act
            var response = bugsController.EditExistingBug(fakeBugToModify.Id, newBugData)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeBugAfterEditing = this.mock.BugRepositoryMock.Object.All().LastOrDefault();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(newBugData.Title, fakeBugAfterEditing.Title);
            Assert.AreEqual(newBugData.Description, fakeBugAfterEditing.Description);
            Assert.AreEqual(newBugData.Status, fakeBugAfterEditing.Status.ToString());
        }

        [TestMethod]
        public void Modify_Existing_Bug_With_InCorrect_Data_Should_Return_400BadRequest_And_Do_Not_Modify_Bug()
        {
            // Arrange
            var fakeBugToModify = this.mock.BugRepositoryMock.Object.All().FirstOrDefault();
            var newBugData = new EditBugBindingModel { };

            var mockContext = new Mock<IBugTrackerData>();
            mockContext.Setup(c => c.Bugs)
                .Returns(this.mock.BugRepositoryMock.Object);

            var bugsController = new BugsController(mockContext.Object);
            this.SetupController(bugsController);
            bugsController.ModelState.AddModelError("No data", "Empty binding model.");

            // Act
            var response = bugsController.EditExistingBug(fakeBugToModify.Id, newBugData)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeBugAfterEditing = this.mock.BugRepositoryMock.Object.All().FirstOrDefault();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(fakeBugToModify.Title, fakeBugAfterEditing.Title);
            Assert.AreEqual(fakeBugToModify.Description, fakeBugAfterEditing.Description);
            Assert.AreEqual(fakeBugToModify.Status, fakeBugAfterEditing.Status);
        }

        [TestMethod]
        public void Modify_NonExisting_Bug_With_Correct_Data_Should_Return_404NotFound()
        {
            // Arrange
            var newBugData = new EditBugBindingModel
            {
                Title = "Modified title",
                Description = "Modified description",
                Status = "Closed"
            };

            var mockContext = new Mock<IBugTrackerData>();
            mockContext.Setup(c => c.Bugs)
                .Returns(this.mock.BugRepositoryMock.Object);

            var bugsController = new BugsController(mockContext.Object);
            this.SetupController(bugsController);

            // Act
            var response = bugsController.EditExistingBug(int.MaxValue, newBugData)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void Modify_Existing_Bug_With_Null_Data_Should_Return_400BadRequest_And_Do_Not_Modify_Bug()
        {
            // Arrange
            var fakeBugToModify = this.mock.BugRepositoryMock.Object.All().FirstOrDefault();

            var mockContext = new Mock<IBugTrackerData>();
            mockContext.Setup(c => c.Bugs)
                .Returns(this.mock.BugRepositoryMock.Object);

            var bugsController = new BugsController(mockContext.Object);
            this.SetupController(bugsController);
            bugsController.ModelState.AddModelError("No data", "Empty binding model.");

            // Act
            var response = bugsController.EditExistingBug(fakeBugToModify.Id, null)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            var fakeBugAfterEditing = this.mock.BugRepositoryMock.Object.All().FirstOrDefault();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(fakeBugToModify.Title, fakeBugAfterEditing.Title);
            Assert.AreEqual(fakeBugToModify.Description, fakeBugAfterEditing.Description);
            Assert.AreEqual(fakeBugToModify.Status, fakeBugAfterEditing.Status);
        }

        private void SetupController(BugsController newsController)
        {
            newsController.Request = new HttpRequestMessage();
            newsController.Configuration = new HttpConfiguration();
        }
    }
}
