namespace BidSystem.Data
{
    using System.Data.Entity;

    using BidSystem.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class BidSystemDbContext : IdentityDbContext<User>
    {
        public BidSystemDbContext()
            : base("BidSystem")
        {
        }
        
        public DbSet<Offer> Offers { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public static BidSystemDbContext Create()
        {
            return new BidSystemDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>()
                .HasRequired<User>(o => o.Seller)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Bid>()
                .HasRequired<User>(b => b.Bidder)
                .WithMany()
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
