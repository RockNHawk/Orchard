#region

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

#endregion

namespace Rijkshuisstijl.PerformanceMonitor
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route("Admin/PerformanceMonitor/{action}",
                        new RouteValueDictionary
                        {
                            {"area", "Rijkshuisstijl.PerformanceMonitor"},
                            {"controller", "PerformanceMonitor"},
                        }, new RouteValueDictionary(),
                        new RouteValueDictionary {{"area", "Rijkshuisstijl.PerformanceMonitor"}},
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor route in GetRoutes())
            {
                routes.Add(route);
            }
        }
    }
}