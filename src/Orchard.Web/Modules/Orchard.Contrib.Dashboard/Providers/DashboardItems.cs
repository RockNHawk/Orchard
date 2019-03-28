using Orchard.Contrib.Dashboard.Services;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Providers
{
    [OrchardFeature("Orchard.Contrib.Dashboard.CommonItems")]
    public class DashboardItems : IDashboardItemProvider
    {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public DashboardItems(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        #region Implementation of IDashboardItemProvider

        public void Describe(DescribeDashboardItemContext context) {
            context.For("CommonItems", T("Common items"), T("Common items"))
                .Item("IFrame", T("IFrame URL"), T("Show URL in IFrame"),
                    _ => Shape.Dashboard_IFrame(Context:_), "IFrameDashboardItem"
                )
                .Item("Static",T("Static text"), T("Show static text"),
                    _ => Shape.Dashboard_Static(Context: _), "StaticDashboardItem"
                );
        }

        #endregion
    }
}