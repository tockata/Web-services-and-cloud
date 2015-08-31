namespace OnlineShop.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using OnlineShop.Data.Contracts;
    using OnlineShop.Models;

    public class MockContainer
    {
        public Mock<IRepository<Ad>> AdRepositoryMock { get; set; }

        public Mock<IRepository<AdType>> AdTypeRepositoryMock { get; set; }

        public Mock<IRepository<ApplicationUser>> UserRepositoryMock { get; set; }

        public Mock<IRepository<Category>> CategoryRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeAds();
            this.SetupFakeAdTypes();
            this.SetupFakeUsers();
            this.SetupFakeCategories();
        }

        private void SetupFakeAds()
        {
            var adTypes = new List<AdType>
            {
                new AdType { Name = "Normal", Index = 100 },
                new AdType { Name = "Premium", Index = 200 }
            };

            var fakeAds = new List<Ad>
            {
                new Ad
                {
                    Id = 5,
                    Name = "Audi A6",
                    Type = adTypes[0],
                    PostedOn = DateTime.Now.AddDays(-6),
                    Owner = new ApplicationUser { UserName = "gosho", Id = "123" },
                    Price = 400
                },
                new Ad
                {
                    Id = 8,
                    Name = "BMW",
                    Type = adTypes[1],
                    PostedOn = DateTime.Now.AddDays(-10),
                    Owner = new ApplicationUser { UserName = "pesho", Id = "111" },
                    Price = 800
                },
                new Ad
                {
                    Id = 3,
                    Name = "VW Golf",
                    Type = adTypes[0],
                    PostedOn = DateTime.Now.AddDays(-9),
                    Owner = new ApplicationUser { UserName = "mimi", Id = "107" },
                    Price = 99
                }
            };

            this.AdRepositoryMock = new Mock<IRepository<Ad>>();
            this.AdRepositoryMock.Setup(r => r.All())
                .Returns(fakeAds.AsQueryable());

            this.AdRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeAds.FirstOrDefault(a => a.Id == id);
                });
        }

        private void SetupFakeAdTypes()
        {
            var fakeAdTypes = new List<AdType>
            {
                new AdType { Name = "Normal", Index = 100, Id = 1 },
                new AdType { Name = "Premium", Index = 200, Id = 2 },
                new AdType { Name = "Gold", Index = 300, Id = 3 }
            };

            this.AdTypeRepositoryMock = new Mock<IRepository<AdType>>();
            this.AdTypeRepositoryMock.Setup(r => r.All())
                .Returns(fakeAdTypes.AsQueryable());

            this.AdTypeRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
            {
                return fakeAdTypes.FirstOrDefault(a => a.Id == id);
            });
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "gosho", Id = "111" },
                new ApplicationUser { UserName = "pesho", Id = "222" },
                new ApplicationUser { UserName = "mimi", Id = "333" }
            };

            this.UserRepositoryMock = new Mock<IRepository<ApplicationUser>>();
            this.UserRepositoryMock.Setup(r => r.All())
                .Returns(fakeUsers.AsQueryable());

            this.UserRepositoryMock.Setup(r => r.Find(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
        }

        private void SetupFakeCategories()
        {
            var fakeCategories = new List<Category>
            {
                new Category { Id = 3, Name = "Cars"},
                new Category { Id = 1, Name = "Phones"},
                new Category { Id = 99, Name = "Cameras"}
            };

            this.CategoryRepositoryMock = new Mock<IRepository<Category>>();
            this.CategoryRepositoryMock.Setup(r => r.All())
                .Returns(fakeCategories.AsQueryable());

            this.CategoryRepositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeCategories.FirstOrDefault(c => c.Id == id);
                });
        }
    }
}
