namespace BookShop.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BookShopSystem.Models;

    public class BookDataModel
    {
        public static Expression<Func<Book, BookDataModel>> DataModel
        {
            get
            {
                return x => new BookDataModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Price = x.Price,
                    Copies = x.Copies,
                    Edition = x.EditionType,
                    AgeRestriction = x.AgeRestriction,
                    ReleaseDate = x.ReleaseDate,
                    Categories = x.Categories.Select(c => c.Name),
                    Author = x.Author.FirstName + " " + x.Author.LastName,
                    AuthorId = x.AuthorId
                };
            }
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public EditionType Edition { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Author { get; set; }

        public Guid AuthorId { get; set; }

        public IEnumerable<string> Categories { get; set; }
    }
}