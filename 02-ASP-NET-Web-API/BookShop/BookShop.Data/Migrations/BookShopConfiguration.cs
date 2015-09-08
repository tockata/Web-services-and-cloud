namespace BookShop.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class BookShopConfiguration : DbMigrationsConfiguration<BookShopEntities>
    {
        public BookShopConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}
