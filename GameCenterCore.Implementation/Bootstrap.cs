using GameCenterCore.Contracts;
using GameCenterCore.Implementation.Repositories;
using GameCenterCore.Implementation.Services;
using GameCenterCore.Repositories;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Implementation
{
    public class Bootstrap
    {
        public static void Init(IUnityContainer container)
        {
            container
                .RegisterType<DbContext, DbContext>(new InjectionConstructor(ConfigurationManager.ConnectionStrings["DefaultConnection2"].ConnectionString))
                
                .RegisterType<IPartyRepository, PartyRepository>()

                .RegisterType<IAuthenticationService, AuthenticationService>()
                .RegisterType<IPartyService, PartyService>()
                ;
        }
    }
}
