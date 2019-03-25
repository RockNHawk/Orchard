using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Amba.HtmlBlocks
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("AppInstanceId:{0}", System.AppDomain.CurrentDomain.Id));

            var appId = T("AppId:{0}", System.AppDomain.CurrentDomain.Id);
            builder.Add(appId, "4",
                          menu => menu.Add(appId, "1.0"));

            builder.Add(T("Html Blocks"), "4",
                        menu => menu
                                    .Add(T("Html Blocks"), "1.0",
                                    item => item.Action("List", "Admin", new { area = "Amba.HtmlBlocks" }))
                                    );
  
        }
    }
}
