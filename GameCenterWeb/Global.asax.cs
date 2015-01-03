using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using AppServer;
using GameCenterWeb.Models;

namespace GameCenterWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            Bootstrap.Init();
        }

        public override void Init()
        {
            base.Init();
            base.PostAuthenticateRequest += ApplicationAuthenticateRequest;
        }

        private void ApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var _application = HttpContext.Current.ApplicationInstance;
            var formsCookie = _application.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (formsCookie != null)
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(formsCookie.Value);

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                CustomIdentitySerialized serializeModel = serializer.Deserialize<CustomIdentitySerialized>(authTicket.UserData);

                if (serializeModel != null)
                {
                    var ci = new CustomIdentity(serializeModel.UserName, serializeModel.PartyId, serializeModel.ConnectionId, serializeModel.RememberMe, serializeModel.Nick, serializeModel.PlayerId );
                    CustomPrincipal newUser = new CustomPrincipal(ci);

                    HttpContext.Current.User = newUser;

                    _application.Context.User = Thread.CurrentPrincipal = newUser;
                }
            }
        }
    }
}