using GameCenterCore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameCenterCore.Contracts.Services;
using GameCenterWeb.Models;
using GameCenterCore.ErrorHandling;
using GameCenterCore.Model;
using AppServer;

namespace GameCenterWeb.Controllers
{
    [Authorize]
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

        IPlayerService _PlayerService;
        IPlayerService PlayerService
        {
            get
            {
                if (_PlayerService == null) _PlayerService = Resolve<IPlayerService>();
                return _PlayerService;
            }
        }

        IAuthenticationService _AuthenticationService;
        IAuthenticationService AuthenticationService
        {
            get
            {
                if (_AuthenticationService == null) _AuthenticationService = Resolve<IAuthenticationService>();
                return _AuthenticationService;
            }
        }

        public ActionResult Index()
        {
            var model = PartyService.GetAllJoinable(base.GetCurrentUserId());
            return View(model);
        }

        public ActionResult Join(Guid partyId)
        {
            IPlayer player = null;
            var results = PartyService.Join(partyId, GetCurrentUserId(), out player);
            var principal = User.Identity as CustomIdentity;
            if (results.IsOk && principal != null)
            {
                var partyUrl = results[0].Context as string;
                AuthenticationService.CreateAuthTicket(principal.Name, partyId, string.Empty, principal.RememberMe, System.Web.HttpContext.Current.Response, principal.Nick, player.Id);
                Response.Redirect(partyUrl);
            }
            else
                ProcessResults(results);
            return new EmptyResult();
        }

        public ActionResult Create()
        {
            var model = new PartyModel { Id = Guid.NewGuid() };
            //model.Players = PlayerService.GetPlayerUsers().Select(x => new UIPlayerModel { Id = Guid.NewGuid(), User = AutoMapper.Mapper.Map<IUser, GameCenterCore.User>(x.User), Nick = x.Nick }).ToList();
            model.Players = PlayerService.GetPlayerUsers().Select(x => new UIPlayerModel { Id = Guid.NewGuid(), UserId = x.UserId, Nick = x.Nick }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(PartyModel model)
        {
            if (ModelState.IsValid)
            {
                IParty party = model.ToParty();
                PartyService.Save(party);
                return RedirectToAction("Index");
            }
            //model.Players = PlayerService.GetPlayerUsers().Select(x => new UIPlayerModel { Id = x.Id, User = AutoMapper.Mapper.Map<GameCenterCore.User>(x.User), Nick = x.Nick }).ToList();
            model.Players = PlayerService.GetPlayerUsers().Select(x => new UIPlayerModel { Id = x.Id, UserId = x.UserId, Nick = x.Nick }).ToList();
            return View("Create", model);
        }

        #region Helper Methods
        IEnumerable<UIPlayerModel> ToUIModell(IEnumerable <IPlayer> players) {
            foreach (var player in players)
            {
                //yield return new UIPlayerModel { Player = player, Selected = false };
            }
            return null;
        }
        #endregion
    }
}
