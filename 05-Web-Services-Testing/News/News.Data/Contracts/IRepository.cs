namespace News.Data.Contracts
{
    using System;
    using System.Linq;

    public interface IRepository<T> : IDisposable where T : class 
    {
        IQueryable<T> All();

        T Find(object id);

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        int SaveChanges();
    }
}
