using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

//[assembly: OwinStartup(typeof(GameCenterWeb.AppServer.Startup))]
//namespace GameCenterWeb.AppServer
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            // Any connection or hub wire up and configuration should go here
//            app.MapSignalR();

//            //var ps = new PathString("/games/mv");

//            //app.Map(ps, map => {
//            //    var hubConf = new HubConfiguration() { };
//            //    map.RunSignalR(hubConf);
//            //} );
//        }
//    }
//}