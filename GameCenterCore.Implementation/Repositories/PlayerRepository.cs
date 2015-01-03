using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;
using GameCenterCore.Implementation.Persistance;
using GameCenterCore.Repositories;

namespace GameCenterCore.Implementation.Repositories
{
    internal class PlayerRepository : RepositoryBase<IPlayer, PlayerDb>, IPlayerRepository
    {
        public new void Update(IPlayer player)
        {
            PlayerDb newRec = Context.Set<PlayerDb>().FirstOrDefault(x => x.Id == player.Id);
            newRec.ConnectionId = player.ConnectionId;
            newRec.Szin = player.Szinkod;
            //newRec.User = AutoMapper.Mapper.Map<UserDb>(player.User);
            newRec.UserId = player.UserId;
            newRec.Nick = player.Nick;
            newRec.ClientStatus = (byte)player.ClientStatus;
            Context.SaveChanges();
        }
    }
}
