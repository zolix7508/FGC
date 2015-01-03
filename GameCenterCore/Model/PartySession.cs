using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.DI;
using Infrastructure.Core.Providers.Session;

namespace GameCenterCore.Model
{
    public class PartySession
    {
        ISession _session;
        protected ISession Session
        {
            get
            {
                if (_session == null) _session = DependencyInjection.Resolve<ISession>();
                return _session;
            }
        }

        private struct SessionKey
        {
            internal const string UserInfo = "UserInfo";
        }

        public UserInfo UserInfo
        {
            get
            {
                return Session.Get<UserInfo>(SessionKey.UserInfo);
            }
            set
            {
                Session.Store(SessionKey.UserInfo, value);
            }
        }
    }
}
