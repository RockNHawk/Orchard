﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Devq.ErrorLog
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var descriptor in GetRoutes())
                routes.Add(descriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "admin/ErrorLog", // url
                        new RouteValueDictionary {
                            {"area", "Devq.ErrorLog"},
                            {"controller", "Admin"},
                            {"action", "Index"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Devq.ErrorLog"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}