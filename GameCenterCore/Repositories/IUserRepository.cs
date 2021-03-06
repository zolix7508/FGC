﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCenterCore.Contracts;

namespace GameCenterCore.Repositories
{
    public interface IUserRepository : IRepository<IUser>
    {
        IUser GetUserByUserName(string userName);
    }
}
