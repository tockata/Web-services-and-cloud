namespace BookShop.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;

    using BookShop.Data;
    using BookShop.Services.Models;

    [RoutePrefix("api/user")]
    public class UsersController : ApiController
    {
        private BookShopEntities context;

        public UsersController()
        {
            this.context = new BookShopEntities();
        }

        [HttpGet]
        [Route("{username}/purchases")]
        public IHttpActionResult GetUSerPurchases(string username)
        {
            var userWithPurchases = this.context.Users
                .Where(u => u.UserName == username)
                .Select(u => new 
                {
                    Username = u.UserName,
                    Purchases = u.Purchases
                    .OrderBy(p => p.PurchaseDate)
                    .Select(p => new PurchaseViewModel
                    {
                        BookTitle = p.Book.Title,
                        Price = p.Price,
                        Buyer = username,
                        IsRecalled = p.IsRecalled,
                        PurchaseDate = p.PurchaseDate
                    })
                })
                .FirstOrDefault();

            return this.Ok(userWithPurchases);
        }
    }
}
