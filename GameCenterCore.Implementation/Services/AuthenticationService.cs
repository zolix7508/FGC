using GameCenterCore;
using GameCenterCore.Contracts;
using GameCenterCore.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace GameCenterCore.Implementation.Services
{
    internal class AuthenticationService : ServiceBase, IAuthenticationService
    {
        public bool Login(string userName, string password, bool rememberMe, out Results results)
        {
            bool ok = WebSecurity.Login(userName, password, persistCookie: rememberMe);
            results = new Results();
            if (!ok) results.Add(ResultCode.LoginError);
            return ok;
        }
    }
}
