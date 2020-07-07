using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ControlPanel.Startup))]
namespace ControlPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
