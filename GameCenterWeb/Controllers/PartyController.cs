using GameCenterCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameCenterWeb.Controllers
{
    public class PartyController : BaseController
    {
        IPartyService _PartyService;
        IPartyService PartyService
        {
            get
            {
                if (_PartyService == null) _PartyService = Resolve<IPartyService>();
                return _PartyService;
            }
        }

        public ActionResult Index()
        {
            PartyService.GetAllPlaying(0);
            return View();
        }

    }
}
