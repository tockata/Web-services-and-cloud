namespace BookShop.Data
{
    using System.Data.Entity;

    using BookShop.Data.Migrations;
    using BookShopSystem.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class BookShopEntities : IdentityDbContext<ApplicationUser>
    {
        public BookShopEntities()
            : base("name=BookShopEntities")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookShopEntities, BookShopConfiguration>());
        }
        
        public IDbSet<Book> Books { get; set; }

        public IDbSet<Category> Categories { get; set; }

        public IDbSet<Author> Authors { get; set; }

        public IDbSet<Purchase> Purchases { get; set; }
        
        public static BookShopEntities Create()
        {
            return new BookShopEntities();
        }
    }
}