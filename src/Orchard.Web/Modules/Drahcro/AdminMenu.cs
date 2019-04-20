using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Drahcro
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
            builder.Add(T("Drahcro"), "4",menu => menu.Add(T("Drahcro"), "1.0"));
        }
    }
}
