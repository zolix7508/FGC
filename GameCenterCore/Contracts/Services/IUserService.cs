using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts.Services
{
    public interface IUserService
    {
        IUser GetUserByUserName(string userName);
    }
}
