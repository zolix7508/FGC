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
using GameCenterCore.Contracts.Services;
using GameCenterCore.Model;
using GameCenterCore.Implementation.Persistance;
using Infrastructure.Core.Providers.Session;
using Infrastructure.Core.IoC;

namespace GameCenterCore.Implementation
{
    public class Bootstrap
    {
        public static void Init(IUnityContainer container)
        {
            container
                .RegisterType<IGame, Game>(new PerResolveLifetimeManager())
                .RegisterType<IParty, Party>(new PerResolveLifetimeManager())
                .RegisterType<IPlayer, Player>(new PerResolveLifetimeManager())
                //.RegisterType<IUser, User>(new PerResolveLifetimeManager())

                .RegisterType<DbContext, DbContext>(new HttpContextLifetimeManager<DbContext>(), new InjectionConstructor(ConfigurationManager.ConnectionStrings["MainPersistanceEntities"].ConnectionString))

                .RegisterType<IPartyRepository, PartyRepository>(new HttpContextLifetimeManager<IPartyRepository>())
                .RegisterType<IPlayerRepository, PlayerRepository>(new HttpContextLifetimeManager<IPlayerRepository>())
                .RegisterType<IUserRepository, UserRepository>(new HttpContextLifetimeManager<IUserRepository>())
                .RegisterType<RepositoryBase<Game, GameDb>, RepositoryBase<Game, GameDb>>(new HttpContextLifetimeManager<RepositoryBase<Game, GameDb>>())
                

                .RegisterType<IAuthenticationService, AuthenticationService>()
                .RegisterType<IGameService, GameService>(new HttpContextLifetimeManager<IGameService>())
                .RegisterType<IPartyService, PartyService>(new HttpContextLifetimeManager<IPartyService>())
                .RegisterType<IPlayerService, PlayerService>(new HttpContextLifetimeManager<IPlayerService>())
                .RegisterType<IUserService, UserService>(new HttpContextLifetimeManager<IUserService>())
                .RegisterType<ISession, Session>()
                ;

            //AutoMapper.Mapper.FindTypeMapFor<IUser, User>();
            AutoMapper.Mapper.CreateMap<IUser, User>();
            AutoMapper.Mapper.CreateMap<User, UserDb>();
            AutoMapper.Mapper.CreateMap<IParty, PartyDb>();
            AutoMapper.Mapper.CreateMap<IPlayer, PlayerDb>();
            AutoMapper.Mapper.CreateMap<PlayerDb, IPlayer>();
            AutoMapper.Mapper.CreateMap<Persistance.UserDb, User>();
            AutoMapper.Mapper.CreateMap<Persistance.UserDb, IUser>();
            AutoMapper.Mapper.CreateMap<Persistance.PartyDb, Party>();
            AutoMapper.Mapper.CreateMap<Persistance.PartyDb, IParty>();
            AutoMapper.Mapper.CreateMap<IGame, GameDb>();
            AutoMapper.Mapper.CreateMap<Persistance.GameDb, Game>();
            AutoMapper.Mapper.CreateMap<Persistance.GameDb, IGame>();
        }
    }
}
