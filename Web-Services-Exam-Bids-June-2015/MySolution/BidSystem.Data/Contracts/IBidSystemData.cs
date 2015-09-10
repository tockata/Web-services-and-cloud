namespace BidSystem.Data.Contracts
{
    using BidSystem.Data.Models;

    public interface IBidSystemData
    {
        IRepository<Bid> Bids { get; }

        IRepository<Offer> Offers { get; }

        IRepository<User> Users { get; }

        int SaveChanges();
    }
}
