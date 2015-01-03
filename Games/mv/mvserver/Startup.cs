using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(mvserver.Startup))]
namespace mvserver
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();

            //var ps = new PathString("/games/mv");

            //app.Map(ps, map => {
            //    var hubConf = new HubConfiguration() { };
            //    map.RunSignalR(hubConf);
            //} );
        }
    }
}
