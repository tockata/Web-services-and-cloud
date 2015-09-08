namespace BugTracker.Data.Contracts
{
    using BugTracker.Data.Models;

    public interface IBugTrackerData
    {
        IRepository<Bug> Bugs { get; }

        IRepository<Comment> Comments { get; }

        IRepository<User> Users { get; }

        int SaveChanges();
    }
}
