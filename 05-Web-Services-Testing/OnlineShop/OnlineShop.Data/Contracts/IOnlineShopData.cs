namespace OnlineShop.Data.Contracts
{
    using OnlineShop.Models;

    public interface IOnlineShopData
    {
        IRepository<Ad> Ads { get; }

        IRepository<AdType> AdTypes { get; }

        IRepository<ApplicationUser> Users { get; }

        IRepository<Category> Categories { get; }

        int SaveChanges();
    }
}
