using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebformTest.Startup))]
namespace WebformTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
