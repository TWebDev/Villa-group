using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ePlatApi.Startup))]

namespace ePlatApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            System.Web.HttpContext.Current.Application["senderAccount"] = 1;
        }
    }
}
