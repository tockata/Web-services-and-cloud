namespace BookShop.Services.Models
{
    using System;
    using System.Collections.Generic;

    using BookShopSystem.Models;

    public class AuthorBooksViewModel
    {
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