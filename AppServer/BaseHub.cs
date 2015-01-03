using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using GameCenterCore.Contracts;
using GameCenterCore.Contracts.Services;
using GameCenterCore.Model;
using Infrastructure.Core.DI;
using Microsoft.AspNet.SignalR;

namespace AppServer
{
    public class BaseHub : Hub
    {
        #region services

        IAuthenticationService _authenticationService;
        IAuthenticationService AuthenticationService
        {
            get
            {
                if (_authenticationService == null) _authenticationService = DependencyInjection.Resolve<IAuthenticationService>();
                return _authenticationService;
            }
        }

        IPartyService _partyService;
        protected IPartyService PartyService
        {
            get
            {
                if (_partyService == null) _partyService = DependencyInjection.Resolve<IPartyService>();
                return _partyService;
            }
        }

        IUserService _userService;
        IUserService UserService
        {
            get
            {
                if (_userService == null) _userService = DependencyInjection.Resolve<IUserService>();
                return _userService;
            }
        }

        IPlayerService _playerService;
        IPlayerService PlayerService
        {
            get
            {
                if (_playerService == null) _playerService = DependencyInjection.Resolve<IPlayerService>();
                return _playerService;
            }
        }
        #endregion

        #region LifeCycle
        public override Task OnConnected()
        {
            Guid partyId;
            IPlayer player = ManageConnection(true, out partyId);
            if (player != null && partyId != Guid.Empty)
            {
                PlayerJoined(partyId.ToString(), player);
            }
            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            Guid partyId;
            IPlayer player = ManageConnection(false, out partyId);
            if (player != null && partyId != Guid.Empty)
            {
                PlayerLeft(partyId.ToString(), player);
            }
            return base.OnDisconnected(stopCalled);
        }


        public override Task OnReconnected()
        {
            Guid partyId;
            IPlayer player = ManageConnection(true, out partyId);
            if (player != null && partyId != Guid.Empty)
            {
                PlayerJoined(partyId.ToString(), player);
            }
            return base.OnReconnected();
        }

        #endregion


        protected async Task PlayerJoined(string groupName, IPlayer player)
        {
            await Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).playerJoined(player.Nick, player.Id);
        }


        protected async Task PlayerLeft(string groupName, IPlayer player)
        {
            Groups.Remove(Context.ConnectionId, groupName);
            Clients.Group(groupName).playerLeft(player.Nick, player.Id);
        }


        protected IPlayer ManageConnection(bool connect, out Guid partyId)
        {
            IPlayer player = null;
            partyId = Guid.Empty;
            //playerIdx = -1;
            if (Context.User != null)
            {
                var principal = Context.User.Identity as CustomIdentity;
                if (principal != null && principal.IsAuthenticated)
                {
                    if (Guid.TryParse(Context.Request.QueryString["partyId"], out partyId) && (principal.PartyId == Guid.Empty || partyId == principal.PartyId))
                    {
                        var user = UserService.GetUserByUserName(principal.Name);
                        var party = PartyService.GetById(partyId);
                        if (user != null && party != null)
                        {
                            IPlayer _player = party.Players.FirstOrDefault(p => p.UserId == user.Id);
                            //_player = party.Players.FirstOrDefault(x=>x.);
                            //var playerUsers = PartyService.GetUsers(partyId);
                            //for (int i=0; i<party.Players.Count(); i++)
                            //{
                            //    var pu = playerUsers.FirstOrDefault(x => x.Id == party.Players.ElementAt(i).UserId);
                            //    if (pu != null) { playerIdx = i; _player = party.Players.ElementAt(playerIdx); break; }
                            //}

                            var newClientStatus = connect ? ClientStatus.Connected : ClientStatus.Offline;
                            if (_player != null)
                            {
                                if (_player.Nick != principal.Nick || _player.ConnectionId != principal.ConnectionId || _player.ClientStatus != newClientStatus || _player.Id != principal.PlayerId)
                                {
                                    _player.Nick = user.Nick;
                                    _player.ConnectionId = Context.ConnectionId;
                                    _player.ClientStatus = newClientStatus;
                                    PlayerService.Update(_player);
                                    //PartyService.Update(party);
                                    if (connect)
                                    {
                                        AuthenticationService.CreateAuthTicket(principal.Name, partyId, Context.ConnectionId, principal.RememberMe, HttpContext.Current.Response, principal.Nick, _player.Id);
                                    }
                                }
                                player = _player;
                            }
                            partyId = party.Id;
                        }
                    }
                }
            }
            return player;
        }
    }
}