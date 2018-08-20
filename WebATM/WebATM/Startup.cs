using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebATM.Startup))]
namespace WebATM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
