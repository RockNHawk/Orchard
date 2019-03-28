using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Orchard.Contrib.Dashboard {
    [OrchardSuppressDependency("Orchard.Core.Dashboard.Routes")]
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Priority = -5,
                                                     Route = new Route(
                                                         "Admin",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Contrib.Dashboard"},
                                                                                      {"controller", "dashboard"},
                                                                                      {"action", "index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Contrib.Dashboard"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = -5,
                                                     Route = new Route(
                                                         "Admin/Dashboard",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Contrib.Dashboard"},
                                                                                      {"controller", "dashboard"},
                                                                                      {"action", "index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Orchard.Contrib.Dashboard"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             //new RouteDescriptor {
                             //                        Priority = -5,
                             //                        Route = new Route(
                             //                            "Admin/DashboardOld",
                             //                            new RouteValueDictionary {
                             //                                                         {"area", "Dashboard"},
                             //                                                         {"controller", "admin"},
                             //                                                         {"action", "index"}
                             //                                                     },
                             //                            new RouteValueDictionary(),
                             //                            new RouteValueDictionary {
                             //                                                         {"area", "Dashboard"}
                             //                                                     },
                             //                            new MvcRouteHandler())
                             //                    }
                         };
        }
    }
}