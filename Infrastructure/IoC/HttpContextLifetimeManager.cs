using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Practices.Unity;

namespace Infrastructure.Core.IoC
{
    /// <summary>
    /// This is a derived type of LifeTimenanager, use it to manage object lifetime within a Http Request Context
    /// see http://cnug.co.in/blogs/shijuv/archive/2008/10/24/asp-net-mvc-tip-dependency-injection-with-unity-application-block.aspx
    /// </summary>
    public class HttpContextLifetimeManager<T> : LifetimeManager, IDisposable
    {
        public override object GetValue()
        {
            return HttpContext.Current != null ? HttpContext.Current.Items[typeof(T).AssemblyQualifiedName] : null;
        }

        public override void RemoveValue()
        {
            if (HttpContext.Current != null) HttpContext.Current.Items.Remove(typeof(T).AssemblyQualifiedName);
        }

        public override void SetValue(object newValue)
        {
            if (HttpContext.Current != null) HttpContext.Current.Items[typeof(T).AssemblyQualifiedName] = newValue;
        }

        public void Dispose()
        {
            RemoveValue();
        }

    }
}
