using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.Model
{
    [Serializable]
    public class UserInfo
    {
        public string UserName;
        public Guid PartyId;
        public string ConnectionId;

        public UserInfo(string userName, Guid partyId)
        {
            this.UserName = userName;
            this.PartyId = partyId;
        }

    }
}
