namespace BookShop.Services.Controllers
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Http;
    using System.Web.OData;

    using BookShop.Data;
    using BookShop.Services.Models;

    using BookShopSystem.Models;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BookShopEntities context;

        public BooksController()
        {
            this.context = new BookShopEntities();
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBook(string id)
        {
            Guid guidId = new Guid(id);
            var book = this.context.Books
                .Where(b => b.Id == guidId)
                .Select(BookDataModel.DataModel)
                .FirstOrDefault();

            if (book == null)
            {
                return this.NotFound();
            }

            return this.Ok(book);
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<SearchBookViewModel> SearchForBooks([FromUri]string search)
        {
            var booksResults = this.context.Books
                .Where(b => b.Title.Contains(search) || b.Description.Contains(search))
                .OrderBy(b => b.Title)
                .Take(10)
                .Select(b => new SearchBookViewModel
                {
                    Id = b.Id.ToString(),
                    Title = b.Title
                });

            return booksResults;
        }

        [HttpPost]
        public IHttpActionResult PostBook(AddBookBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newBook = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Copies = model.Copies,
                EditionType = model.Edition,
                AgeRestriction = model.AgeRestriction,
                ReleaseDate = model.ReleaseDate
            };

            var authorId = new Guid(model.AuthorId);
            var author = this.context.Authors
                .FirstOrDefault(a => a.Id == authorId);

            if (author == null)
            {
                return this.BadRequest("Invalid author id");
            }

            newBook.AuthorId = authorId;

            char[] separator = new char[] { ' ' };
            string[] categories = model.Categories.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var categoryName in categories)
            {
                var categoryInDb = this.context.Categories
                    .FirstOrDefault(c => c.Name == categoryName);

                if (categoryInDb == null)
                {
                    Category newCategory = new Category { Name = categoryName };
                    this.context.Categories.Add(newCategory);
                    this.context.SaveChanges();
                    categoryInDb = newCategory;
                }

                newBook.Categories.Add(categoryInDb);
            }

            this.context.Books.Add(newBook);
            this.context.SaveChanges();
            var bookToReturn = this.context.Books
                .Where(b => b.Id == newBook.Id)
                .Select(BookDataModel.DataModel)
                .FirstOrDefault();

            return this.Ok(bookToReturn);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditBook(string id, EditBookBindingModel model)
        {
            Guid bookId = new Guid(id);
            var bookInDb = this.context.Books
                .FirstOrDefault(b => b.Id == bookId);

            if (bookInDb == null)
            {
                return this.BadRequest("This book does not extists!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            bookInDb.Title = model.Title;
            bookInDb.Description = model.Description;
            bookInDb.Price = model.Price;
            bookInDb.Copies = model.Copies;
            bookInDb.EditionType = model.Edition;
            bookInDb.AgeRestriction = model.AgeRestriction;
            bookInDb.ReleaseDate = model.ReleaseDate;

            var authorId = new Guid(model.AuthorId);
            var author = this.context.Authors
                .FirstOrDefault(a => a.Id == authorId);

            if (author == null)
            {
                return this.BadRequest("Invalid author id");
            }

            bookInDb.AuthorId = authorId;
            
            this.context.Books.AddOrUpdate(bookInDb);
            this.context.SaveChanges();
            var bookToReturn = this.context.Books
                .Where(b => b.Id == bookInDb.Id)
                .Select(BookDataModel.DataModel)
                .FirstOrDefault();

            return this.Ok(bookToReturn);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBook(string id)
        {
            Guid bookId = new Guid(id);
            var bookInDb = this.context.Books
                .FirstOrDefault(b => b.Id == bookId);

            if (bookInDb == null)
            {
                return this.BadRequest("This book does not extists!");
            }

            this.context.Books.Remove(bookInDb);
            this.context.SaveChanges();

            return this.Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult BuyBook(string id)
        {
            var bookId = new Guid(id);
            var book = this.context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return this.NotFound();
            }

            if (book.Copies < 1)
            {
                return this.BadRequest("Not enough copies of this book.");
            }

            string currentUserId = this.User.Identity.GetUserId();
            Purchase newPurchase = new Purchase
            {
                ApplicationUserId = currentUserId,
                BookId = bookId,
                IsRecalled = false,
                Price = book.Price,
                PurchaseDate = DateTime.Now
            };

            this.context.Purchases.Add(newPurchase);
            book.Copies = book.Copies - 1;
            this.context.Books.AddOrUpdate(book);
            this.context.SaveChanges();

            var buyerUsername = this.context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.UserName)
                .FirstOrDefault();

            return this.Ok(new PurchaseViewModel
            {
                BookTitle = book.Title,
                Price = book.Price,
                PurchaseDate = newPurchase.PurchaseDate,
                IsRecalled = newPurchase.IsRecalled,
                Buyer = buyerUsername
            });
        }

        [Authorize]
        [HttpPut]
        [Route("recall/{id}")]
        public IHttpActionResult RecallPurchase(string id)
        {
            var bookId = new Guid(id);
            var book = this.context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Identity.GetUserId();
            var purchase = this.context.Purchases
                .Where(p => p.ApplicationUserId == currentUserId && p.BookId == bookId && p.IsRecalled == false)
                .OrderByDescending(p => p.PurchaseDate)
                .FirstOrDefault();

            if (purchase == null)
            {
                return this.NotFound();
            }

            purchase.IsRecalled = true;
            book.Copies = book.Copies + 1;
            this.context.Purchases.AddOrUpdate(purchase);
            this.context.Books.AddOrUpdate(book);
            this.context.SaveChanges();

            return this.Ok(string.Format("Purchase with id: {0} succesfully recalled.", purchase.Id));
        }
    }
}
