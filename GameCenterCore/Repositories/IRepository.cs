using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        void Save(T entity);
        //T GetById<TPersisted>(Expression<Func<TPersisted, Guid>> selector, Guid key) where TPersisted : class;
        T GetById(Guid key);
        //List<T> GetAllFor<Expression<Func<TKey>>>();
        //IEnumerable<T> GetAllFor<TKey>(Expression<Func<T, TKey>> selector, TKey key);
        void Update<TKey>(T entity, TKey key);
        void Update(T entity);
    }
}
