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
        public void AddParty(Guid key, mvParty party)
        {
            lock (obj)
            {
                mvPartik.Add(key, party);
            }
        }


        public void GetStatus(Guid partyId)
        {
            mvParty mvParty;
            var party = GetPartyStatus(partyId, out mvParty);
            Clients.Caller.setStatus(party.Data);
        }

        public void szinSelected(byte kod)
        {
            //var szin = mvSzin.GetSzin(kod);
            var user = Context.User.Identity as CustomIdentity;
            if (user != null && kod != null)
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
            Guid partyId; mvParty mvParty;
            var user = Context.User.Identity as CustomIdentity;
            Guid playerId = user != null ? user.PlayerId : Guid.Empty;
            if (Guid.TryParse(Context.Request.QueryString["partyId"], out partyId))
            {
                var party = GetPartyStatus(partyId, out mvParty);
                if (party != null) Clients.Caller.setStatus(party.Data, playerId);
            }
        }

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
