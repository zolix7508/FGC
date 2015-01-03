using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameCenterCore.Contracts;
using Infrastructure.Core.DI;

namespace GameCenterWeb.Models
{
    public class PartyModel
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Name { get; set; }
        public List<UIPlayerModel> Players { get; set; }

        public PartyModel() { this.Players = new List<UIPlayerModel> {}; }

        internal IParty ToParty()
        {
            IParty party = DependencyInjection.Resolve<IParty>();
            party.GameId = this.GameId;
            party.StatusId = PartyStatus.Created;
            party.Id = this.Id;
            if (this.Players != null)
                party.Players = GetPlayers(this.Players);
            return party;
        }

        protected IEnumerable<IPlayer> GetPlayers(IEnumerable<UIPlayerModel> uiPlayers)
        {
            foreach (var uiPlayer in uiPlayers.Where(p=>p.Selected))
            {
                var player = DependencyInjection.Resolve<IPlayer>();
                player.Id = uiPlayer.Id;
                player.UserId = uiPlayer.UserId;
                yield return player;
            }
        }
    }
}