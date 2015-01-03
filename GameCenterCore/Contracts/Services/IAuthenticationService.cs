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
        void CreateAuthTicket(string userName, Guid partyId, string connectionId, bool RememberMe, object response, string nick, Guid playerId);
    }
}
