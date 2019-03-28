using System;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Events;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Providers
{
    public interface IFormProvider : IEventHandler
    {
        void Describe(dynamic context);
    }
    [OrchardFeature("Orchard.Contrib.Dashboard.CommonItems")]
    public class DashboardForms : IFormProvider
    {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public DashboardForms(IShapeFactory shapeFactory)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }
        #region Implementation of IFormProvider

        public void Describe(dynamic context) {
            Func<IShapeFactory, object> form =
                shape => Shape.Form(
                Id: "IFrameDashboardItem",
                _DisplayName:Shape.Textbox(
                    Id: "displayName", Name: "displayName",
                    Title: T("Display Name"),
                    Description: T("Item header."),
                    Classes: new[] { "textMedium" }),
                _Url: Shape.Textbox(
                    Id: "url", Name: "url",
                    Title: T("IFrame URL"),
                    Description: T("This url will be used as IFrame target."),
                    Classes: new[] { "textMedium" }),
                _TechnicalName: Shape.Textbox(
                    Id: "technicalName", Name: "technicalName",
                    Title: T("Technical Name"),
                    Description: T("Technical Name used in class and id."),
                    Classes: new[] { "textMedium" })
                );
            context.Form("IFrameDashboardItem", form);

            form =
                shape => Shape.Form(
                Id: "StaticDashboardItem",
                _DisplayName: Shape.Textbox(
                    Id: "displayName", Name: "displayName",
                    Title: T("Display Name"),
                    Description: T("Item header."),
                    Classes: new[] { "textMedium" }),
                _Message: Shape.Textarea(
                    Id: "Body", Name: "Body",
                    Title: T("Body"),
                    Description: T("The body of the item.")),
                _TechnicalName: Shape.Textbox(
                    Id: "technicalName", Name: "technicalName",
                    Title: T("Technical Name"),
                    Description: T("Technical Name used in class and id."),
                    Classes: new[] { "textMedium" })
                );
                context.Form("StaticDashboardItem", form);
        }

        #endregion
    }
}