using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Seo;
using System.Linq;

namespace Nop.Plugin.ExtraAbility.Infrastructure
{
    public class GenericUrlRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
           
             //and default one
            //routeBuilder.MapRoute("Default", "{controller}/{action}/{id?}");

            //generic URLs
            routeBuilder.MapGenericPathRoute("jio", "{GenericSeName}",
                new { controller = "Common", action = "GenericUrl" });
            //Cusstom GeneriPathRout.cs 
            routeBuilder.MapLocalizedRoute("Prooduct", "{SeName}",
               new { controller = "ProductCustomer", action = "ProductDetails" });
            //Cusstom GeneriPathRout.cs 
            routeBuilder.MapLocalizedRoute("Catajolog", "{SeName}",
                new { controller = "Catalog", action = "Category" });
            //Cusstom GeneriPathRout.cs 
            routeBuilder.MapLocalizedRoute("Venhudor", "{SeName}",
                new { controller = "Catalog", action = "Vendor" });

            //routeBuilder.MapLocalizedRoute("BlogPosts", "{SeName}",
            //   new { controller = "ExtraBlog", action = "BlogPosts" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            //it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
            get { return 0; }
        }

        #endregion
    }
}
