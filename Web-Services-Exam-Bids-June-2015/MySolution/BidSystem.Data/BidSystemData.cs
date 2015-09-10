namespace BidSystem.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    using BidSystem.Data.Contracts;
    using BidSystem.Data.Models;

    public class BidSystemData : IBidSystemData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public BidSystemData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Bid> Bids
        {
            get { return this.GetRepository<Bid>(); }
        }

        public IRepository<Offer> Offers
        {
            get { return this.GetRepository<Offer>(); }
        }

        public IRepository<User> Users
        {
            get { return this.GetRepository<User>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }
    }
}
