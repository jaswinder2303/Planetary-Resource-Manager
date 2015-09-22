using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(startupType:(typeof(PlanetaryResourceManager.Api.SignalR.Startup)))]
namespace PlanetaryResourceManager.Api.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}