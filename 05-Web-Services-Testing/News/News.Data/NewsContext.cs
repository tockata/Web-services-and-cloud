namespace News.Data
{
    using System.Data.Entity;

    using News.Data.Migrations;
    using News.Models;

    public class NewsContext : DbContext
    {
        public NewsContext()
            : base("name=NewsContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsContext, NewsConfiguration>());
        }

        public virtual IDbSet<News> News { get; set; }
    }
}