using GameCenterCore.Contracts;
using GameCenterCore.ErrorHandling;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameCenterCore.Contracts.Services;
using Infrastructure.Core.Providers.Session;
using GameCenterWeb.Models;
using GameCenterCore.Model;
using Infrastructure.Core.DI;

namespace GameCenterWeb.Controllers
{
    public class BaseController : AsyncController
    {
        //protected Results _results = new Results();

        protected T Resolve<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        protected void ProcessResults(Results results)
        {
            foreach (var result in results.Where(r => r.IsError))
                ModelState.AddModelError(String.Empty, result.Message);
        }

        IUserService _UserService;
        protected IUserService UserService
        {
            get
            {
                if (_UserService == null) _UserService = Resolve<IUserService>();
                return _UserService;
            }
        }

        protected int GetCurrentUserId()
        {
            IUser user = this.UserService.GetUserByUserName(User.Identity.Name);
            return user != null ? user.Id : 0;
        }

        IAuthenticationService _authenticationService;
        IAuthenticationService AuthenticationService
        {
            get
            {
                if (_authenticationService == null) _authenticationService = DependencyInjection.Resolve<IAuthenticationService>();
                return _authenticationService;
            }
        }

        //PartySession _session;
        //protected new PartySession Session
        //{
        //    get
        //    {
        //        if (_session == null) _session = new PartySession();
        //        return _session;
        //    }
        //}


    }
}
