namespace BugTracker.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BugTracker.Data.Contracts;
    using BugTracker.Data.Models;

    using Moq;

    public class MockContainer
    {
        public Mock<IRepository<Bug>> BugRepositoryMock { get; set; }

        public void PrepareMock()
        {
            var fakeBugs = new List<Bug>
            {
                new Bug
                {
                    Id = 5,
                    Title = "Bug 5",
                    Description = "Description 5",
                    Status = Status.Open,
                    DateCreated = new DateTime(2015, 01, 01)
                },
                new Bug
                {
                    Id = 1,
                    Title = "Bug 1",
                    Description = "Description 1",
                    Status = Status.Closed,
                    DateCreated = new DateTime(2014, 01, 01)
                },
                new Bug
                {
                    Id = 99,
                    Title = "Bug 99",
                    Description = "Description 99",
                    Status = Status.Fixed,
                    DateCreated = new DateTime(2015, 05, 15)
                }
            };

            this.BugRepositoryMock = new Mock<IRepository<Bug>>();
            this.BugRepositoryMock.Setup(r => r.All())
                .Returns(fakeBugs.AsQueryable());

            this.BugRepositoryMock.Setup(r => r.Update(It.IsAny<Bug>()))
                .Callback((Bug bug) =>
                {
                    var bugToUpdate = fakeBugs.FirstOrDefault(b => b.Id == bug.Id);
                    fakeBugs.Remove(bugToUpdate);
                    fakeBugs.Add(bug);
                });
        }
    }
}
