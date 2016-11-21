using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FYP_6.Startup))]
namespace FYP_6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
