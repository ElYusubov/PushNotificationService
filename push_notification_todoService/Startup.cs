using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(push_notification_todoService.Startup))]

namespace push_notification_todoService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}