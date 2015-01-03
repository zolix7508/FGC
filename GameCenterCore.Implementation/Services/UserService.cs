using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts.Services;
using GameCenterCore.Repositories;

namespace GameCenterCore.Implementation.Services
{
    internal class UserService : ServiceBase, IUserService
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

        public Contracts.IUser GetUserByUserName(string userName)
        {
            return UserRepository.GetAll().Where(u => u.UserName == userName).FirstOrDefault();
        }
    }
}
