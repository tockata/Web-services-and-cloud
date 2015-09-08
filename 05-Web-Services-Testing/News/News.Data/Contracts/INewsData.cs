namespace News.Data.Contracts
{
    using News.Models;

    public interface INewsData
    {
        IRepository<News> News { get; }

        int SaveChanges();
    }
}
