using System;
using System.Web.Mvc;
using StaticEmbedded;
using System.Web.Routing;
using System.Web.Optimization;
using System.Web.Hosting;
[assembly: WebActivator.PostApplicationStartMethod(typeof(AppStart), "Start")]

namespace StaticEmbedded
{
    public static class AppStart
    {
        public static void Start()
        {
            ConfigureRoutes();
            ConfigureBundles();
        }

        private static void ConfigureBundles()
        {
            BundleTable.VirtualPathProvider = new EmbeddedVirtualPathProvider(HostingEnvironment.VirtualPathProvider);

            BundleTable.Bundles.Add(new ScriptBundle("~/StaticEmbedded/Embedded/Js")
                //農金js
                .Include("~/StaticEmbedded/Embedded/decimal.js")
                .Include(
                "~/StaticEmbedded/Embedded/jquery-3.6.0.js",
                //"~/StaticEmbedded/Embedded/jquery-3.6.0.slim.js",
                "~/StaticEmbedded/Embedded/jquery.cookie.js",
                "~/StaticEmbedded/Embedded/jquery.validate.js",
                "~/StaticEmbedded/Embedded/jquery.validate.unobtrusive.js",
                "~/StaticEmbedded/Embedded/jquery.unobtrusive-ajax.js"
                )

                .Include(
                "~/StaticEmbedded/Embedded/jquery-ui-1.13.1.js",
                "~/StaticEmbedded/Embedded/jquery.blockUI.js"
                )

                 .Include(
                "~/StaticEmbedded/Embedded/bootstrap-datepicker.js",
                "~/StaticEmbedded/Embedded/bootstrap-datepicker.min.js"
                )
                );
            BundleTable.Bundles.Add(new StyleBundle("~/StaticEmbedded/Embedded/Css")
                .Include("~/StaticEmbedded/Embedded/NewTextBox1.css")
                .Include("~/StaticEmbedded/Embedded/NewTextBox2.css")
                );
        }

        private static void ConfigureRoutes()
        {
            RouteTable.Routes.Insert(0,
                new Route("StaticEmbedded/Embedded/{file}.{extension}",
                    new RouteValueDictionary(new { }),
                    new RouteValueDictionary(new { extension = "css|js" }),
                    new EmbeddedResourceRouteHandler()
                ));
        }
    }
}