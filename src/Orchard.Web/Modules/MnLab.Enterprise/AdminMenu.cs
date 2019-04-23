using Orchard.Environment;
using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace MnLab.PdfVisualDesign.Binding
{
    public class AdminMenu : INavigationProvider
    {
        private readonly Work<RequestContext> _requestContextAccessor;
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }
        public AdminMenu(Work<RequestContext> requestContextAccessor) {
            _requestContextAccessor = requestContextAccessor;
            T = NullLocalizer.Instance;
        }
        public void GetNavigation(NavigationBuilder builder) {

            var requestContext = _requestContextAccessor.Value;
            var idValue = (string)requestContext.RouteData.Values["id"];
            var id = 0;

            if (!string.IsNullOrEmpty(idValue)) {
                int.TryParse(idValue, out id);
            }


            // Image set 图标显示
            //.AddImageSet("说明书管理")

            // "说明书管理"
            builder.Add(item => item

                    .Caption(T("说明书管理"))
                    .Position("2")
                    .LinkToFirstChild(false)

                    // "说明书管理"
                    .Add(subItem => subItem
                        .Caption(T("说明书管理"))
                        .Position("1.0")
                        .Action(new RouteValueDictionary
                        {
                            {"area", "MnLab.Enterprise"},
                            {"controller", "InstructionsAdmin"},
                            {"action", "Instructions"}
                        })

                        .Add(T("Details"), i => i.Action("Edit", "InstructionsAdmin", new { id }).LocalNav())
                        .Add(T("Addresses"), i => i.Action("ListAddresses", "InstructionsAdmin", new { id }).LocalNav())
                        .Add(T("Orders"), i => i.Action("ListOrders", "InstructionsAdmin", new { id }).LocalNav())
                    )
                    // "审批列表"
                    .Add(subItem => subItem
                        .Caption(T("审批列表"))
                        .Position("1.0")
                        .Action(new RouteValueDictionary
                        {
                            {"area", "MnLab.Enterprise"},
                            {"controller", "ApprovalAdmin"},
                            {"action", "ApprovalList"}
                        })
                    )
                );
        }
        //public void GetNavigation(NavigationBuilder builder)
        //{

        //    //builder.Add(T("AppInstanceId:{0}",System.AppDomain.CurrentDomain.Id));

        //    builder.Add(T("Approval"), "4",menu => menu.Add(T("Approval"), "1.0"));

        //}
    }
}
