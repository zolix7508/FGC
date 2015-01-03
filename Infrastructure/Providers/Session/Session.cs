using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Core.Providers.Session
{
    public class Session :ISession
    {
        public bool Exists()
        {
            return HttpContext.Current != null && HttpContext.Current.Session != null;
        }

        public T Get<T>(string key)
        {
            return (T)(HttpContext.Current.Session[key]);
        }

        public void Store(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        public void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }
    }
}
