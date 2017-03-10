using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CampanasDelDesierto_v1.Startup))]
namespace CampanasDelDesierto_v1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
