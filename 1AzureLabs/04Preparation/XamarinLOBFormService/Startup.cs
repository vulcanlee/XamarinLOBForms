using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(XamarinLOBFormService.Startup))]

namespace XamarinLOBFormService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}