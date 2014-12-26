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
        //List<T> GetAllFor<Expression<Func<TKey>>>();
        //IEnumerable<T> GetAllFor<TKey>(Expression<Func<T, TKey>> selector, TKey key);
    }
}
