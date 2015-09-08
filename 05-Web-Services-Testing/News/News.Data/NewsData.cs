namespace News.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    using News.Data.Contracts;
    using News.Models;

    public class NewsData : INewsData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public NewsData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<News> News
        {
            get { return this.GetRepository<News>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(EfRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }
    }
}
