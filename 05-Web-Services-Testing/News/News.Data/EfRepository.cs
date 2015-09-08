namespace News.Data
{
    using System.Data.Entity;
    using System.Linq;

    using News.Data.Contracts;

    public class EfRepository<T> : IRepository<T> where T : class 
    {
        protected DbContext context;

        protected DbSet<T> set;

        public EfRepository(DbContext context)
        {
            this.context = context;
            this.set = context.Set<T>();
        }

        public IQueryable<T> All()
        {
            return this.set.AsQueryable();
        }

        public T Find(object id)
        {
            return this.set.Find(id);
        }

        public T Add(T entity)
        {
            this.ChangeState(entity, EntityState.Added);
            return entity;
        }

        public void Update(T entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void Delete(T entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }

        private void ChangeState(T entity, EntityState state)
        {
            var entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.set.Attach(entity);
            }

            entry.State = state;
        }
    }
}
