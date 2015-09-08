namespace News.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class NewsConfiguration : DbMigrationsConfiguration<NewsContext>
    {
        public NewsConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(NewsContext context)
        {
        }
    }
}
