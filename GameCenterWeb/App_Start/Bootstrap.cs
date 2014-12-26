using GameCenterCore.Contracts;
using GameCenterCore.Implementation;
using GameCenterCore.Repositories;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace GameCenterWeb
{
    public class Bootstrap
    {
        public static void Init()
        {
            RegisterComponents();
        }

        private static void RegisterComponents()
        {
            var container = new UnityContainer();

            GameCenterCore.Implementation.Bootstrap.Init(container);

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }
    }
}