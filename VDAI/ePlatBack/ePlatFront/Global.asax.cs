using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Net;

namespace ePlatFront
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            BundleTable.EnableOptimizations = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Web.HttpContext.Current.Application["senderAccount"] = 2;
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            string culture = ePlatBack.Models.Utils.GeneralFunctions.GetCulture();
            if (culture != null)
            {
                string language = culture;
                //string language = culture.Substring(0, 2);
                CultureInfo ci = new CultureInfo(language);
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
                System.Threading.Thread.CurrentThread.CurrentCulture =
                CultureInfo.CreateSpecificCulture(ci.Name);
            }
        }
    }
}