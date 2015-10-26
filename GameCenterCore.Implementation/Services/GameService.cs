using GameCenterCore.Contracts;
using GameCenterCore.Contracts.Services;
using GameCenterCore.Implementation.Persistance;
using GameCenterCore.Implementation.Repositories;
using GameCenterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Implementation.Services
{
    internal class GameService: ServiceBase, IGameService
    {
        private RepositoryBase<Game, GameDb> _repository;

        private RepositoryBase<Game, GameDb> GameRepository
        {
            get
            {
                if (_repository == null) _repository = Resolve<RepositoryBase<Game, GameDb>>();
                return _repository;
            }
        }


        public IGame GetById(Guid id)
        {
            return this.GameRepository.GetById(x => x.Id == id);
        }
    }
}
