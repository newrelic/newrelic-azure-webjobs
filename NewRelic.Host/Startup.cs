using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NewRelic.Host.Startup))]
namespace NewRelic.Host
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
