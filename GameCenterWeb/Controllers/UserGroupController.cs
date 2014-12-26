using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameCenterWeb.Controllers
{
    [Authorize]
    public class UserGroupController : BaseController
    {
        public ActionResult Index(int? userId)
        {
            //IUserGroupService 
            return View();
        }

    }
}
