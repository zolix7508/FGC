using GameCenterCore.Contracts;
using GameCenterCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return PartyRepository.GetAll().Where(x => x.IsRunning && x.Players.Any(pl => pl.UserId == userId));
        }
    }
}
