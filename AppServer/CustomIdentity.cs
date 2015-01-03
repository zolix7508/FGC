using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AppServer
{
    [Serializable]
    public class CustomIdentity : IIdentity
    {
        private readonly string _userName;
        private readonly Guid _partyId;
        private readonly string _nick;
        private readonly Guid _playerId;
        public string ConnectionId { get; set; }
        public bool RememberMe { get; set; }

        public CustomIdentity(string name, Guid partyId, string connectionId, bool rememberMe, string nick, Guid playerId)
        {
            _userName = name.Trim();
            _partyId = partyId;
            ConnectionId = connectionId;
            _nick = nick;
            _playerId = playerId;
        }


        public string AuthenticationType
        {
            get { return "CustomIdentity"; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(_userName); }
        }

        public string Name
        {
            get { return _userName; }
        }

        public Guid PartyId
        {
            get { return _partyId; }
        }

        public Guid PlayerId
        {
            get { return _playerId; }
        }

        public string Nick
        {
            get { return _nick; }
        }
    }
}