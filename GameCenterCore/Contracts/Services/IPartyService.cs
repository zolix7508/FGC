using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IPartyService
    {
        IEnumerable<IParty> GetAllPlaying(int userId);
        IEnumerable<IParty> GetAllJoinable(int userId);

        void Save(IParty party);

        ErrorHandling.Results Join(Guid partyId, int userId);
        IParty GetById(Guid partyId);
        void Update(IParty party);

        IEnumerable<IUser> GetUsers(Guid partyId);
    }
}
