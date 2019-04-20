using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Drahcro.Web;
using Orchard.Localization;

namespace Drahcro.Web.Drivers {
    public class WebCoreSupportPartDriver : ContentPartDriver<WebCoreSupportPart> {

        private const string TemplateName = "Parts.WebCoreSupport";

        public Localizer T { get; set; }

        //protected override string Prefix {
        //    get { return "WebCoreSupport"; }
        //}

        protected override DriverResult Display(WebCoreSupportPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_WebCoreSupport",
                    () => shapeHelper.Parts_WebCoreSupport()),
                ContentShape("Parts_WebCoreSupport_Summary",
                    () => shapeHelper.Parts_WebCoreSupport_Summary()),
                ContentShape("Parts_WebCoreSupport_SummaryAdmin",
                    () => shapeHelper.Parts_WebCoreSupport_SummaryAdmin())
                );
        }

        protected override DriverResult Editor(WebCoreSupportPart part, dynamic shapeHelper) {

            return ContentShape("Parts_WebCoreSupport_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(WebCoreSupportPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        //protected override void Importing(WebCoreSupportPart part, ImportContentContext context) {
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null) {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "Approval", Approval =>
        //        part.CommitOpinion = Approval
        //    );
        //}

        //protected override void Exporting(WebCoreSupportPart part, ExportContentContext context) {
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Approval", part.CommitOpinion);
        //}
    }
}