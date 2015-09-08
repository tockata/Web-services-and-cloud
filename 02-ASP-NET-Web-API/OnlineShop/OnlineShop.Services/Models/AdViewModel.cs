namespace OnlineShop.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using OnlineShop.Models;

    public class AdViewModel
    {
        public static Expression<Func<Ad, AdViewModel>> Create
        {
            get
            {
                return a => new AdViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    AdOwner = new UserViewModel
                    {
                        Id = a.OwnerId,
                        Username = a.Owner.UserName
                    },
                    Type = a.Type.Name,
                    PostedOn = a.PostedOn,
                    Categories = a.Categories.Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                };
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public UserViewModel AdOwner { get; set; }

        public string Type { get; set; }

        public DateTime PostedOn { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}