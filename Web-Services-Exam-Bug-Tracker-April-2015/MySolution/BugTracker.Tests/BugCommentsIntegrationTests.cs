namespace BugTracker.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;

    using BugTracker.RestServices.Models.Comments;
    using BugTracker.Tests.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BugCommentsIntegrationTests
    {
        [TestMethod]
        public void Get_Comments_For_Existing_GivenBug_Should_Return200Ok_And_All_Comments()
        {
            // Arrange -> create a new bug with two comments
            TestingEngine.CleanDatabase();

            var bugTitle = "bug title";
            var httpPostResponse = TestingEngine.SubmitBugHttpPost(bugTitle, null);
            Assert.AreEqual(HttpStatusCode.Created, httpPostResponse.StatusCode);
            var submittedBug = httpPostResponse.Content.ReadAsAsync<BugModel>().Result;

            var httpPostResponseFirstComment =
                TestingEngine.SubmitCommentHttpPost(submittedBug.Id, "Comment 1");
            Assert.AreEqual(HttpStatusCode.OK, httpPostResponseFirstComment.StatusCode);
            Thread.Sleep(2);

            var httpPostResponseSecondComment =
                TestingEngine.SubmitCommentHttpPost(submittedBug.Id, "Comment 2");
            Assert.AreEqual(HttpStatusCode.OK, httpPostResponseSecondComment.StatusCode);

            // Act
            var response = TestingEngine.HttpClient.GetAsync("api/bugs/" + submittedBug.Id + "/comments").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var commentsFromService = response.Content.ReadAsAsync<List<BugDetailsCommentViewModel>>().Result;
            Assert.AreEqual(2, commentsFromService.Count);
            Assert.IsTrue(commentsFromService[0].Id > 0);
            Assert.AreEqual("Comment 2", commentsFromService[0].Text);
            Assert.AreEqual(null, commentsFromService[0].Author);
            Assert.IsTrue(commentsFromService[0].DateCreated - DateTime.Now < TimeSpan.FromMinutes(1));

            Assert.IsTrue(commentsFromService[1].Id > 0);
            Assert.AreEqual("Comment 1", commentsFromService[1].Text);
            Assert.AreEqual(null, commentsFromService[1].Author);
            Assert.IsTrue(commentsFromService[1].DateCreated - DateTime.Now < TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public void Get_Comments_For_NonExisting_Bug_Should_Return404NotFound()
        {
            // Arrange -> create a new bug with two comments
            TestingEngine.CleanDatabase();

            // Act
            var response = TestingEngine.HttpClient.GetAsync("api/bugs/" + int.MaxValue + "/comments").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
