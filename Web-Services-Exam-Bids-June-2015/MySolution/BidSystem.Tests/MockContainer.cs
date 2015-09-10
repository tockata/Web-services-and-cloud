namespace BidSystem.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BidSystem.Data.Contracts;
    using BidSystem.Data.Models;

    using Moq;

    public class MockContainer
    {
        public Mock<IRepository<Bid>> BidRepositoryMock { get; set; }

        public Mock<IRepository<User>> UserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeBids();
            this.SetupFakeUsers();
        }

        private void SetupFakeBids()
        {
            var fakeUsers = new List<User>
            {
                new User { UserName = "gosho", Id = "111" },
                new User { UserName = "pesho", Id = "222" },
                new User { UserName = "mimi", Id = "333" }
            };

            var fakeBids = new List<Bid>
            {
                new Bid
                {
                    Id = 99, 
                    Bidder = fakeUsers[0],
                    BidderId = "111",
                    BidPrice = 100,
                    Comment = "Gogsho`s bid comment",
                    Date = DateTime.Now.AddDays(-5),
                    OfferId = 1
                },
                new Bid
                {
                    Id = 5, 
                    Bidder = fakeUsers[2],
                    BidderId = "333",
                    BidPrice = 200,
                    Comment = "other user bid comment",
                    Date = DateTime.Now.AddDays(-4),
                    OfferId = 3
                },
                new Bid
                {
                    Id = 55, 
                    Bidder = fakeUsers[0],
                    BidderId = "111",
                    BidPrice = 123,
                    Comment = "Gogsho`s bid comment",
                    Date = DateTime.Now.AddDays(-7),
                    OfferId = 1
                },
                new Bid
                {
                    Id = 33, 
                    Bidder = fakeUsers[1],
                    BidderId = "222",
                    BidPrice = 300,
                    Comment = "other user bid comment",
                    Date = DateTime.Now.AddDays(-3),
                    OfferId = 2
                }
            };

            this.BidRepositoryMock = new Mock<IRepository<Bid>>();
            this.BidRepositoryMock.Setup(r => r.All())
                .Returns(fakeBids.AsQueryable());
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<User>
            {
                new User { UserName = "gosho", Id = "111" },
                new User { UserName = "pesho", Id = "222" },
                new User { UserName = "mimi", Id = "333" }
            };

            this.UserRepositoryMock = new Mock<IRepository<User>>();
            this.UserRepositoryMock.Setup(r => r.All())
                .Returns(fakeUsers.AsQueryable());

            this.UserRepositoryMock.Setup(r => r.Find(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
        }

    }
}
