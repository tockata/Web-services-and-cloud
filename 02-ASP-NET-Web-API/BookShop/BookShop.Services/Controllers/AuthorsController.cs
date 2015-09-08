namespace BookShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Services.Models;

    using BookShopSystem.Models;

    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private BookShopEntities context;

        public AuthorsController()
        {
            this.context = new BookShopEntities();
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetAuthorById(string id)
        {
            var guidId = new Guid(id);
            var author = this.context.Authors.Find(guidId);
            if (author == null)
            {
                return this.NotFound();
            }

            return this.Ok(author);
        }

        [HttpPost]
        public IHttpActionResult PostAuthor(AddAuthorBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            Author newAuthor = new Author { FirstName = model.FirstName, LastName = model.LastName };
            this.context.Authors.Add(newAuthor);
            this.context.SaveChanges();
            return this.Ok(newAuthor);
        }

        [HttpGet]
        [EnableQuery]
        [Route("{id}/books")]
        public IQueryable<AuthorBooksViewModel> GetAuthorBooks(string id)
        {
            var guidId = new Guid(id);
            var author = this.context.Authors.Find(guidId);

            if (author == null)
            {
                return null;
            }

            var books = author.Books
                .Select(b => new AuthorBooksViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Price = b.Price,
                    Copies = b.Copies,
                    Edition = b.EditionType,
                    AgeRestriction = b.AgeRestriction,
                    ReleaseDate = b.ReleaseDate,
                    Categories = b.Categories.Select(c => c.Name),
                    Author = b.Author.FirstName + " " + b.Author.LastName,
                    AuthorId = b.AuthorId
                })
                .AsQueryable();

            return books;
        }
    }
}
