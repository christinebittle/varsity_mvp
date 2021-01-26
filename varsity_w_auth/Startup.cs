using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(varsity_w_auth.Startup))]
namespace varsity_w_auth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
