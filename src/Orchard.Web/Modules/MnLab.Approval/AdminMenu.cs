using Orchard.Localization;
using Orchard.UI.Navigation;

namespace MnLab.PdfVisualDesign.Binding
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

            //builder.Add(T("AppInstanceId:{0}",System.AppDomain.CurrentDomain.Id));

            builder.Add(T("Approval"), "4",menu => menu.Add(T("Approval"), "1.0"));

        }
    }
}
