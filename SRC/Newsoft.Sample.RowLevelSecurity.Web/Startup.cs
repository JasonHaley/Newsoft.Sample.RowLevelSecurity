using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Newsoft.Sample.RowLevelSecurity.Web.Startup))]
namespace Newsoft.Sample.RowLevelSecurity.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
