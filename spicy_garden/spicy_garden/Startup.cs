using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(spicy_garden.Startup))]
namespace spicy_garden
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
