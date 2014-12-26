using GameCenterCore.Repositories;
using Infrastructure.Core.DI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Implementation.Repositories
{
    internal class RepositoryBase<T> : IRepository<T> where T : class
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
            return Context.Set<T>();
        }

        //public IEnumerable<T> GetAllFor<TKey>(Expression<Func<T, TKey>> selector, TKey key)
        //{
        //    IEnumerable<T> a = null;
        //    return a.Where(x => selector.Compile()(x).Equals(key)).ToList();
        //}
    }
}
