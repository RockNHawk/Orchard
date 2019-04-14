using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using MnLab.Approval.Models;
using Orchard.Localization;

namespace MnLab.Approval.Drivers {
    public class ApprovalPartDriver : ContentPartDriver<ApprovalPart> {

        private const string TemplateName = "Parts.ApprovalPart";

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "Approval"; }
        }

        protected override DriverResult Display(ApprovalPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_ApprovalPart",
                    () => shapeHelper.Parts_ApprovalPart(Approval: part.UserCommit)),
                ContentShape("Parts_ApprovalPart_Summary",
                    () => shapeHelper.Parts_ApprovalPart_Summary(Approval: part.UserCommit)),
                ContentShape("Parts_ApprovalPart_SummaryAdmin",
                    () => shapeHelper.Parts_ApprovalPart_SummaryAdmin(Approval: part.UserCommit))
                );
        }

        protected override DriverResult Editor(ApprovalPart part, dynamic shapeHelper) {

            return ContentShape("Parts_ApprovalPart_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ApprovalPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Importing(ApprovalPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Approval", Approval =>
                part.UserCommit = Approval
            );
        }

        protected override void Exporting(ApprovalPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Approval", part.UserCommit);
        }
    }
}