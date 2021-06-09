using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ePlatBack
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Dashboard", // Route name
                "dashboard/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "CRM-Reports", // Route name
                "crm/reports/get/{report}", // URL with parameters
                new { controller = "Reports", action = "Index", report = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "CRM-Reports2", // Route name
            "settings/catalogs2/get/{catalog}", // URL with parameters
            new { controller = "Catalogs", action = "Index2", catalog = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "CRM-Manifest", // Route name
                "crm/reports2/get/{report}", // URL with parameters
                new { controller = "Reports", action = "Index2", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "CRM", // Route name
                "crm/{controller}/{action}/{id}", // URL with parameters
                new { controller = "MasterChart", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "CMS", // Route name
                "cms/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Activities", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Settings-Catalogs", // Route name
                "settings/catalogs/get/{catalog}", // URL with parameters
                new { controller = "Catalogs", action = "Index", catalog = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Settings-Catalogs2", // Route name
                "settings/catalogs2/get/{catalog}", // URL with parameters
                new { controller = "Catalogs", action = "Index2", catalog = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Settings", // Route name
                "settings/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Users", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


            routes.MapRoute(
                "Membership", // Route name
                "membership/{controller}/{action}/{id}", // URL with parameters
                new { controller = "cardsManagement", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Content", // Route name
                "Content/{controller}/{action}/{id}", // URL with parameters
                new { controller = "content", action = "Hotels", id = UrlParameter.Optional } // Parameter defaults
            );


            /* routes.MapRoute(
                 "Settings-Catalogs", // Route name
                 "settings/catalogs/get/{catalog}", // URL with parameters
                 new { controller = "Catalogs", action = "Index2", catalog = UrlParameter.Optional } // Parameter defaults
             );*/

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}