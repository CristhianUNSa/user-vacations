using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserVacations.Startup))]
namespace UserVacations
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
