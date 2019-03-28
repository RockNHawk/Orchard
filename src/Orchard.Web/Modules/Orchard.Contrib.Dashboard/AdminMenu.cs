using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.Contrib.Dashboard
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Settings"), 
                    menu => menu.Add(T("Dashboard"), "10",
                        item => item
                            .Action("Index", "DashboardAdmin", new { area = "Orchard.Contrib.Dashboard" })
                            .Permission(StandardPermissions.SiteOwner)));
        }
    }
}