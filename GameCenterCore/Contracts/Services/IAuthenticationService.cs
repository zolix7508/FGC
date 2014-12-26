using GameCenterCore.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Contracts
{
    public interface IAuthenticationService
    {
        bool Login (string userName, string password, bool rememberMe, out Results results);
    }
}
