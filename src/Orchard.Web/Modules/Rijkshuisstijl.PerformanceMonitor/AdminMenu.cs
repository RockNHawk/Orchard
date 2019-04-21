#region Using

using Orchard;
using Orchard.Security;
using Orchard.UI.Navigation;

#endregion

namespace Rijkshuisstijl.PerformanceMonitor
{
    public class AdminMenu : Component, INavigationProvider
    {
        public string MenuName
        {
            get { return "admin"; }
        }

        //public void GetNavigation(NavigationBuilder builder)
        //{
        //    builder
        //        .Add(T("Rijkshuisstijl"), "2", LinkSubMenu);
        //}

        //private void LinkSubMenu(NavigationBuilder menu)
        //{
        //    menu.Add(item => item
        //        .Position("21")
        //        .Caption(T("Performance Monitor"))
        //        .Action("Index", "PerformanceMonitor", new { area = "Rijkshuisstijl.PerformanceMonitor" })
        //        .Permission(StandardPermissions.SiteOwner));
        //}

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
              .Add(T("Performance Monitor"), "13",
                  menu => menu.Add(T("Performance Monitor"), "13",
                  item => item.Action("Index", "PerformanceMonitor", new { area = "Rijkshuisstijl.PerformanceMonitor" })
              .Permission(StandardPermissions.SiteOwner)));
        }

    }
}