using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.DI
{
    public class DependencyInjection
    {
        public static T Resolve<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}
