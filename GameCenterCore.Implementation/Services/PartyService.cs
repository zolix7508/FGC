using GameCenterCore.Contracts;
using GameCenterCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Model;
using GameCenterCore.ErrorHandling;
using Infrastructure.Core.ErrorHandling;

namespace GameCenterCore.Implementation.Services
{
    internal class PartyService : ServiceBase, IPartyService
    {
        private IPartyRepository _PartyRepository;

        private IPartyRepository PartyRepository
        {
            get {
                if (_PartyRepository == null) _PartyRepository = Resolve<IPartyRepository>();
                return _PartyRepository;
            }
        }


        public IEnumerable<IParty> GetAllPlaying(int userId)
        {
            return PartyRepository.GetAll().Where(x => x.IsRunning && x.Players.Any(pl => pl.UserId  == userId));
        }

        public IEnumerable<IParty> GetAllJoinable(int userId)
        {
            return PartyRepository.GetAll().Where(x => (x.IsRunning || x.StatusId == PartyStatus.Created) && x.Players.Any(pl => pl.UserId == userId));
        }

        public void Save(IParty party)
        {
            PartyRepository.Save(party);
        }

        public Results Join(Guid partyId, int userId)
        {
            var results = new Results();
            IParty party = this.GetAllJoinable(userId).Where(p => p.Id == partyId).FirstOrDefault();
            if (party != null)
            {
                var players = party.Players.Where(p => p.UserId == userId);
                if (players.Count() == 1)
                {
                    var player = players.ElementAt(0);
                    results.Add(ResultCode.Undefined, @"\Games\mv\map.html?partyId=" + partyId);
                }
            }
            else
            {
                results.Add(ResultCode.PartyNotFound);
            }
            return results;
        }

        public IParty GetById(Guid partyId)
        {
            return PartyRepository.GetById(partyId);
            
        }

        public void Update(IParty party)
        {
            PartyRepository.Update(party);
        }


        public IEnumerable<IUser> GetUsers(Guid partyId)
        {
            return PartyRepository.GetUsers(partyId);
        }
    }
}
