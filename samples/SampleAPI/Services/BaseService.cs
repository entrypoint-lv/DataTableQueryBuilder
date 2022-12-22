using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace SampleAPI.Services
{
    using Models;

    public abstract class BaseService<T> where T : class
    {
        protected IDataContext dataContext;
        protected readonly DbSet<T> dbset;

        protected BaseService(IDataContext dataContext)
        {
            this.dataContext = dataContext;
            dbset = dataContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            dbset.Add(entity);
            dataContext.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            dbset.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;

            dataContext.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            dbset.Remove(entity);

            dataContext.SaveChanges();
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();

            foreach (T obj in objects)
                dbset.Remove(obj);

            dataContext.SaveChanges();
        }

        public virtual T GetById(int id)
        {
            return dbset.Find(id);
        }

        public virtual T GetById(string id)
        {
            return dbset.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return dbset;
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).First<T>();
        }
    }
}
