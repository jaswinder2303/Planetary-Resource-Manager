using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(startupType:(typeof(PlanetaryResourceManager.Api.SignalR.Startup)))]
namespace PlanetaryResourceManager.Api.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}