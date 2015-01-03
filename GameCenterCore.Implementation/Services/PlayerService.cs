using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;
using GameCenterCore.Contracts.Services;
using GameCenterCore.Repositories;

namespace GameCenterCore.Implementation.Services
{
    internal class PlayerService : ServiceBase, IPlayerService
    {
        private IUserRepository _UserRepository;

        private IUserRepository UserRepository
        {
            get
            {
                if (_UserRepository == null) _UserRepository = Resolve<IUserRepository>();
                return _UserRepository;
            }
        }

        private IPlayerRepository _PlayerRepository;

        private IPlayerRepository PlayerRepository
        {
            get
            {
                if (_PlayerRepository == null) _PlayerRepository = Resolve<IPlayerRepository>();
                return _PlayerRepository;
            }
        }

        public IEnumerable<IPlayer> GetPlayerUsers()
        {
            foreach (IUser user in UserRepository.GetAll())
            {
                IPlayer player = base.Resolve<IPlayer>();
                player.Nick = user.Nick;
                player.UserId = user.Id;
                yield return player;
            }
        }

        public void Update(IPlayer player)
        {
            PlayerRepository.Update(player);
        }
    }
}
