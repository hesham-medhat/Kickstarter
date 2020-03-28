using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kickstarter.Startup))]
namespace Kickstarter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
