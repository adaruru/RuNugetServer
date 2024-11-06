using StaticEmbedded;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTest
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Insert(0,
                new Route("StaticEmbedded/Embedded/{file}.{extension}",
                    new RouteValueDictionary(new { }),
                    new RouteValueDictionary(new { extension = "css|js" }),
                    new EmbeddedResourceRouteHandler()
                ));
            routes.Add(new Route("StaticEmbedded/Embedded/NewTextBox2.css", new EmbeddedResourceRouteHandler()));

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
