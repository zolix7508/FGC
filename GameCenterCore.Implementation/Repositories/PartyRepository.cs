using GameCenterCore.Contracts;
using GameCenterCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Implementation.Persistance;

namespace GameCenterCore.Implementation.Repositories
{
    internal class PartyRepository : RepositoryBase<IParty, PartyDb>, IPartyRepository
    {
        public new IParty GetById(Guid key)
        {
            //return base.GetById((PartyDb p) => p.Id, key);
            var partyDb = base.Context.Set<PartyDb>().FirstOrDefault(p => p.Id == key);
            return AutoMapper.Mapper.Map<IParty>(partyDb);
        }

        public new void Update(IParty party)
        {
            PartyDb newRec = Context.Set<PartyDb>().FirstOrDefault(x => x.Id == party.Id);
            newRec.Data = party.Data;
            newRec.StatusId = (short)party.StatusId;
            newRec.UpdatedDateUTC = DateTime.UtcNow;
            newRec.Version = party.Version;
            Context.SaveChanges();
        }

        public IEnumerable<IUser> GetUsers(Guid partyId)
        {
            IEnumerable<UserDb> users = Context.Set<PartyDb>().Where(p => p.Id == partyId)
                .SelectMany(p => p.Players).Select(u => u.User);

           return AutoMapper.Mapper.Map<IEnumerable<IUser>>(users);
        }
    }
}
