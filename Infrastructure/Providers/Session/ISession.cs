using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Providers.Session
{
    public interface ISession
    {
        T Get<T>(string key);
        void Store(string key, object value);
        void Remove(string key);
        void Clear();
        void Abandon();
        bool Exists();
    }
}
