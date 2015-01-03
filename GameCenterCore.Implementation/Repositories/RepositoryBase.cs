using GameCenterCore.Repositories;
using Infrastructure.Core.DI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity.Validation;

namespace GameCenterCore.Implementation.Repositories
{
    internal class RepositoryBase<T, TPersisted> : IRepository<T> where T : class where TPersisted :class 
    {
        private DbContext _context;

        protected RepositoryBase() { }

        protected DbContext Context
        {
            get
            {
                if (_context == null) _context = DependencyInjection.Resolve<DbContext>();
                return _context;
            }
        }

        public List<T> GetAllOf<TKey>(Expression<Func<T, TKey>> selector, TKey key)
            where TKey : IEquatable<TKey>
        {
            IEnumerable<T> a = null;
            a.Where(x=> selector.Compile()(x).Equals(key));
            return null;
        }

        public IEnumerable<T> GetAll()
        {
            var res = Context.Set<TPersisted>();
            return AutoMapper.Mapper.Map<IEnumerable<TPersisted>, IEnumerable<T>>(res);
        }

        //public IEnumerable<T> GetAllFor<TKey>(Expression<Func<T, TKey>> selector, TKey key)
        //{
        //    IEnumerable<T> a = null;
        //    return a.Where(x => selector.Compile()(x).Equals(key)).ToList();
        //}

        public void Save(T entity)
        {
            TPersisted dbEntity = AutoMapper.Mapper.Map<T, TPersisted>(entity);
            Context.Set<TPersisted>().Add(dbEntity);
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errs = ex.EntityValidationErrors.ToArray();
            }
        }

        public T GetById(Expression<Func<TPersisted, bool>> filter)
        {
            if (filter != null)
            {
                var del = filter.Compile();
                TPersisted result = Context.Set<TPersisted>().FirstOrDefault(del);
                return AutoMapper.Mapper.Map<T>(result);
            }
            return default(T);
        }

        //public T GetById(Expression<Func<TPersisted, string>> selector, string key)
        //{
        //    return this.GetById<string>(selector, key);
        //}

        //public T GetById<TPersisted>(Expression<Func<TPersisted, Guid>> selector, Guid key) where TPersisted : class
        //{
        //    if (selector != null)
        //    {
        //        var del = selector.Compile();
        //        TPersisted result = Context.Set<TPersisted>().FirstOrDefault(x => del(x).Equals(key));
        //        return AutoMapper.Mapper.Map<T>(result);
        //    }
        //    return default(T);
        //}

        //public T GetById(Expression<Func<TPersisted, Guid>> selector, Guid key)
        //{
        //    if (selector != null)
        //    {
        //        var del = selector.Compile();
        //        TPersisted result = Context.Set<TPersisted>().FirstOrDefault(x => del(x).Equals(key));
        //        return AutoMapper.Mapper.Map<T>(result);
        //    }
        //    return default(T);
        //}

        public T GetById(Guid key) { return default(T); }

        public void Update<TKey>(T item, TKey key) {
            //this.Update<TKey>(item, selector, key);
         }

        public void Update(T item) { }

        public void Update<TKey>(T item, Expression<Func<TPersisted, TKey>> selector, TKey key)
        {
            var del = selector.Compile();
            var dbItem = Context.Set<TPersisted>().FirstOrDefault(x => del(x).Equals(key));
            AutoMapper.Mapper.Map(item, dbItem);
            //var entry = Context.Entry(dbItem);
            //entry.Reload();
            //if (entry.State == EntityState.Detached)
            //{
            //    Context.Set<TPersisted>().Remove(dbItem);
            //    Context.Set<TPersisted>().Attach(dbItem);
            //}
            //entry.State = EntityState.Modified;
            Context.SaveChanges();
        }
    }
}
