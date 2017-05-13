using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KKU_DEMO
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "LogIn", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //   name: "GetShifts",
            //   url: "{controller}/{action}/{factoryid}/{date}",
            //   defaults: new { controller = "Shift", action = "GetShifts", factoryid = 1, date = "" }
            // );
        
          
        }
    }
}
