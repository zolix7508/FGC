using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppServer
{
    public class CustomIdentitySerialized
    {
        public string UserName { get; set; }
        public Guid PartyId { get; set; }
        public Guid PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public bool RememberMe { get; set; }
        public string Nick { get; set; }
    }
}