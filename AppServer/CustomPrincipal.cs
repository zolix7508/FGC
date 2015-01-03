using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AppServer
{
    public sealed class CustomPrincipal : IPrincipal
    {
        private readonly CustomIdentity _identity;

        public CustomPrincipal(CustomIdentity identity)
        {
            _identity = identity;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return false;
            //return _identity != null &&
            //   _identity.IsAuthenticated &&
            //   !string.IsNullOrWhiteSpace(role) &&
            //   Roles.IsUserInRole(_identity.Name, role);
        }

        //public CustomIdentity Identity
        //{
        //    get { return _identity; }
        //}
    }
}