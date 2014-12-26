using GameCenterCore.Contracts;
using GameCenterCore.ErrorHandling;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    }
}
