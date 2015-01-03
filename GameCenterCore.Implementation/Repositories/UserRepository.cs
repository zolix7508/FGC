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
    internal class UserRepository : RepositoryBase<IUser, UserDb>, IUserRepository
    {
        public IUser GetUserByUserName(string userName)
        {
            var userDb = base.Context.Set<UserDb>().FirstOrDefault(u => u.UserName == userName);
            return AutoMapper.Mapper.Map<IUser>(userDb);
        }
    }
}
