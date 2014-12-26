using GameCenterCore.Contracts;
using GameCenterCore.Implementation.Persistance;
using GameCenterCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Implementation.Repositories
{
    internal class PartyRepository : RepositoryBase<IParty>, IPartyRepository
    {
        public PartyRepository(DbContext context)
        {
        }
    }
}
