using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ePlatFront
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Ignore("{*allfiles}", new { allfiles = @".*(\.ashx|\.zip|\.rar|\.exe|\.htc|\.gif|\.png|\.jpg|\.ico|\.pdf|\.css|\.js)(\?.*)?" });

            routes.MapRoute(
                name: "Not Found", // Route name
                url: "not-found", // URL with parameters
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "Home", // Route name
                url: "home/{action}/{id}", // URL with parameters
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "Confirmation", // Route name
                url: "purchase/confirmation", // URL with parameters
                defaults: new { controller = "Purchase", action = "Confirmation", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "Controls", // Route name
                url: "controls/{action}/{id}", // URL with parameters
                defaults: new { controller = "Controls", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "Page",
                url: "pages/{id}",
                defaults: new { controller = "Pages", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Page V2",
                url: "pagesV2/{id}",
                defaults: new { controller = "Pages", action = "IndexV2", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Packages List DVH3",
               url: "packagesV3/{destination}",
               defaults: new { controller = "Packages", action = "IndexV3", destination = "" }
           );
            
            routes.MapRoute(
               name: "Packages List DVH2",
               url: "packagesV2/{destination}",
               defaults: new { controller = "Packages", action = "IndexV2", destination = "" }
           );

            routes.MapRoute(
                name: "Packages List",
                url: "packagesV1/{destination}",
                defaults: new { controller = "Packages", action = "Index", destination = "" }
            );

            routes.MapRoute(
                name: "Packages List Español",
                url: "paquetes/{destination}",
                defaults: new { controller = "Packages", action = "Index", destination = "" }
            );

            routes.MapRoute(
               name: "Package DVH3",
               url: "packagesV4/detail/{id}",
               defaults: new { controller = "Packages", action = "DetailV4", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Package DVH2",
               url: "packagesV2/detail/{id}",
               defaults: new { controller = "Packages", action = "DetailV2", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Package TGA2",
               url: "packagesV3/detail/{id}",
               defaults: new { controller = "Packages", action = "DetailV3", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Package Brief",
               url: "packages/brief/{id}",
               defaults: new { controller = "Packages", action = "Brief", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Package Detail",
                url: "packagesV1/{destination}/{package}",
                defaults: new { controller = "Packages", action = "Detail", destination = "", package = "" }
            );

            routes.MapRoute(
                name: "Package Detail Español",
                url: "paquetes/{destination}/{package}",
                defaults: new { controller = "Packages", action = "Detail", destination = "", package = "" }
            );

            routes.MapRoute(
                name: "Package Methods",
                url: "packagesController/{action}/{id}",
                defaults: new { controller = "Packages", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Activities Methods",
                url: "activities/{action}/{id}",
                defaults: new { controller = "Activities", action = "Detail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Private Label Methods",
                url: "privatelabel/{action}/{id}",
                defaults: new { controller = "PrivateLabel", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Register To Win",
                url: "registertowin/{action}/{id}",
                defaults: new { controller = "RegisterToWin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Purchase",
                url: "purchase/{action}/{id}",
                defaults: new { controller = "Purchase", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Coupons",
                url: "coupons/{coupon}",
                defaults: new { controller = "Coupons", action = "Index", coupon = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Coupon Bar Code",
                url: "coupons/{action}/{id}",
                defaults: new { controller = "Coupons", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Save Survey Method",
                url: "survey/SaveSurvey",
                defaults: new { controller = "Survey", action = "SaveSurvey" }
            );

            routes.MapRoute(
                name: "Survey",
                url: "survey/{id}",
                defaults: new { controller = "Survey", action = "Index", coupon = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Survey Methods",
                url: "survey/{action}/{id}",
                defaults: new { controller = "Survey", action = "Index", id = UrlParameter.Optional }                 
            );

            routes.MapRoute(
                name: "Router", // Route name
                url: "{*path}", // URL with parameters
                defaults: new { controller = "Home", action = "Router", path = "" } // Parameter defaults
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}