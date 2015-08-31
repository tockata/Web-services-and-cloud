namespace OnlineShop.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    using OnlineShop.Data.Contracts;
    using OnlineShop.Models;

    public class OnlineShopData : IOnlineShopData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public OnlineShopData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Ad> Ads
        {
            get { return this.GetRepository<Ad>(); }
        }

        public IRepository<AdType> AdTypes
        {
            get { return this.GetRepository<AdType>(); }
        }

        public IRepository<ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }

        public IRepository<Category> Categories
        {
            get { return this.GetRepository<Category>(); }
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
