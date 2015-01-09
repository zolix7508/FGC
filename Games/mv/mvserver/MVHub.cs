using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServer;
using GameCenterCore.Contracts;
using Infrastructure.Core.Serialization;

namespace mvserver
{
    public class MVHub : BaseHub
    {
        private static readonly object obj = new object();
        private static readonly Dictionary<Guid, mvParty> mvPartik = new Dictionary<Guid, mvParty>();

        private CustomIdentity _user;
        private CustomIdentity user
        {
            get
            {
                if (_user == null) _user = Context.User.Identity as CustomIdentity;
                return _user;
            }
        }

        private Guid? _requestPartyId;
        Guid RequestPartyId {
            get {
                if (!_requestPartyId.HasValue) {
                    Guid partyId;
                    if (!Guid.TryParse(Context.Request.QueryString["partyId"], out partyId))
                        partyId = Guid.Empty;

                    _requestPartyId = partyId;
                }
                return _requestPartyId.Value;
            }
        }


        public void GetStatus(Guid partyId)
        {
            mvParty mvParty;
            var party = GetPartyStatus(partyId, out mvParty);
            Clients.Caller.setStatus(party.Data);
        }


        public void babuClicked(short b)
        {
            if (user != null)
            {
                IParty party = null;
                mvParty mvp = mvPartik[user.PartyId];
                var resp = CheckCurrentPlayer(mvp);
                if (resp.isEmpty)
                    resp = mvp.babuClicked(b);
                if (resp.StatusChanged)
                {
                    party = PartyService.GetById(user.PartyId);
                    party.Data = Serialize.ToJson(mvp);
                    PartyService.Update(party);
                }
                SendResponse(resp, party);
            }
        }


        public void action(int idx)
        {
            IParty party = null;
            mvParty mvp = mvPartik[user.PartyId];
            var resp = CheckCurrentPlayer(mvp);
            if (resp.isEmpty)
                resp = mvp.Action(idx);
            if (resp.StatusChanged)
            {
                party = PartyService.GetById(user.PartyId);
                party.Data = Serialize.ToJson(mvp);
                PartyService.Update(party);
            }
            SendResponse(resp, party);
        }

        public void szinSelected(byte kod)
        {
            //var szin = mvSzin.GetSzin(kod);
            var user = Context.User.Identity as CustomIdentity;
            if (user != null)
            {
                mvParty mvp = mvPartik[user.PartyId];
                var player = mvp.players.FirstOrDefault(p => p.Id == user.PlayerId);
                if (player != null)
                {
                    player.Szinkod = kod;
                    mvp.szabadSzinek = mvp.getSzinek().Except(mvp.players.Select(p => p.Szinkod)).ToList();
                    
                    var party = PartyService.GetById(user.PartyId);
                    party.Data = Serialize.ToJson(mvp);
                    PartyService.Update(party);

                    Clients.Group(user.PartyId.ToString()).setStatus(party.Data);
                }
            }
        }

        public void SzinMehet()
        {
            var user = Context.User.Identity as CustomIdentity;
            if (user != null)
            {
                mvParty mvp = mvPartik[user.PartyId];
                var player = mvp.players.FirstOrDefault(p => p.Id == user.PlayerId);
                if (player != null)
                {
                    player.szinMehet = true;

                    if (!mvp.players.Any(p => !p.szinMehet)) mvp.StartPlaying();

                    var party = PartyService.GetById(user.PartyId);
                    party.Data = Serialize.ToJson(mvp);
                    PartyService.Update(party);

                    Clients.Group(user.PartyId.ToString()).setStatus(party.Data);
                }
            }
        }

        public void sendMessage(object message)
        {
            Clients.All.messageReceived(message);
        }

        #region Helper Methods
        protected void AddParty(Guid key, mvParty party)
        {
            lock (obj)
            {
                mvPartik.Add(key, party);
            }
        }

        mvResponse CheckClient()
        {
            var resp = new mvResponse();
            if (user == null)
                resp.AddMsgItem("notLoggedIn");
            else if (user.PartyId != RequestPartyId)
                resp.AddMsgItem("notYourParty");

            return resp;
        }

        mvResponse CheckCurrentPlayer(mvParty mvp)
        {
            var resp = CheckClient();
            if (resp.isEmpty)
                if (mvp == null || mvp.CurrentPlayerIdx < 0 || mvp.players[mvp.CurrentPlayerIdx].Id != user.PlayerId)
                    resp.AddMsgItem("notYourTurn");

            return resp;
        }

        void SendResponse(mvResponse resp, IParty party)
        {
            if (user == null) return;
            var groupName = user.PartyId.ToString();
            var items = resp.GetItems();

            foreach (mvResponseMsgItem item in items.OfType<mvResponseMsgItem>())
            {
                Clients.Group(groupName).msg(item.Message);
            }


            foreach (mvResponseActionItem item in items.OfType<mvResponseActionItem>())
            {
                var args = item.Items;
                switch (item.ActionKind)
                {
                    case ActionKind.FullStatus:
                        if (party != null)
                            Clients.Group(groupName).setStatus(party.Data);
                        break;
                    case ActionKind.SelectBabu:
                        Clients.Group(groupName).processBabu(args[0]);
                        break;
                    case ActionKind.DrawBabuk:
                        Clients.Group(groupName).drawBabuk(args[0], args[1], args[2]);
                        break;
                    case ActionKind.UpdateCurrentPlayer:
                        Clients.Group(groupName).updateCurrentPlayer(args[0], args[1], args[2]);
                        break;
                    case ActionKind.TileRemoved:
                        Clients.Group(groupName).tileRemoved(args[0], args[1]);
                        break;
                    case ActionKind.RemoveIsolated:
                        Clients.Group(groupName).removeIsolated(args[0], args[1]);
                        break;
                    case ActionKind.RemoveBabu:
                        Clients.Group(groupName).removeBabu(args[0]);
                        break;
                    case ActionKind.LadaDeployed:
                        Clients.Group(groupName).ladaDeployed(args[0], args[1]);
                        break;
                    case ActionKind.WinterStart:
                        Clients.Group(groupName).winterStart(args[0]);
                        break;
                }
            }
        }

        void SendToCaller(mvResponse resp)
        {
            foreach (mvResponseMsgItem item in resp.GetItems().OfType<mvResponseMsgItem>())
            {
                Clients.Caller.msg(item.Message);
            }
        }

        
        protected IParty GetPartyStatus(Guid partyId, out mvParty mvParty)
        {
            mvParty = null;
            var party = base.PartyService.GetById(partyId);
            if (party != null)
            {
                lock (obj)
                {
                    if (party.Data == null)
                    {
                        mvParty = new mvParty { NX = 6, NY = 21 };
                        mvParty.Init(party.Players);
                        party.Data = Serialize.ToJson(mvParty);
                        PartyService.Update(party);
                    }
                    else
                    {
                        var playerUsers = base.PartyService.GetUsers(partyId);
                        mvParty = Serialize.FromJson<mvParty>(party.Data);
                        foreach (var puser in playerUsers)
                        {
                            var partyPlayer = party.Players.FirstOrDefault(p => p.UserId == puser.Id);
                            if (partyPlayer != null) { partyPlayer.Nick = puser.Nick; }
                            var mvPartyPlayer = mvParty.players.FirstOrDefault(p => p.UserId == puser.Id);
                            if (mvPartyPlayer != null) { mvPartyPlayer.Nick = puser.Nick; mvPartyPlayer.ClientStatus = partyPlayer.ClientStatus; }
                        }
                        party.Data = Serialize.ToJson(mvParty);
                        PartyService.Update(party);
                    }

                    if (mvPartik.ContainsKey(partyId))
                        mvPartik[partyId] = mvParty;
                    else
                        AddParty(partyId, mvParty);
                }
            }
            return party;
        }

        protected void OnConnect(Task t)
        {
            var resp = CheckClient();
            if (resp.isEmpty)
            {
                mvParty mvParty;
                Guid partyId = RequestPartyId;
                Guid playerId = user != null ? user.PlayerId : Guid.Empty;
                if (partyId != Guid.Empty)
                {
                    var party = GetPartyStatus(partyId, out mvParty);
                    if (party != null) Clients.Caller.setStatus(party.Data, playerId);
                }
            }
            else
                SendToCaller(resp);
        }

        #endregion

        public override Task OnConnected()
        {
            return base.OnConnected().ContinueWith(t => OnConnect(t));
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected().ContinueWith(t => OnConnect(t));
        }
    }
}
