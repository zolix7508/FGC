using GameCenterCore;
using GameCenterCore.Contracts;
using GameCenterCore.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMatrix.WebData;
using AppServer;
using System.Web.Security;
using System.Web;
using System.Web.Script.Serialization;
using GameCenterCore.Repositories;

namespace GameCenterCore.Implementation.Services
{
    internal class AuthenticationService : ServiceBase, IAuthenticationService
    {
        public bool Login(string userName, string password, bool rememberMe, out Results results)
        {
            bool ok = WebSecurity.Login(userName, password, persistCookie: rememberMe);
            results = new Results();
            if (ok)
            {
                var user = base.Resolve<IUserRepository>().GetUserByUserName(userName);
                if (user != null)
                    CreateAuthTicket(userName, Guid.Empty, string.Empty, rememberMe, HttpContext.Current.Response, user.Nick, Guid.Empty);
            }

            if (!ok) results.Add(ResultCode.LoginError);
            return ok;
        }


        public void CreateAuthTicket(string userName, Guid partyId, string connectionId, bool isPersistent, object response, string nick, Guid playerId)
        {
            var cp = new CustomIdentitySerialized { ConnectionId = connectionId, PartyId = partyId, UserName = userName, RememberMe = isPersistent, Nick = nick, PlayerId = playerId };

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(cp);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     userName,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(15),
                     isPersistent,
                     userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            (response as HttpResponse).Cookies.Add(faCookie);

            //return RedirectToAction("Index", "Home");
        }
    }
}
