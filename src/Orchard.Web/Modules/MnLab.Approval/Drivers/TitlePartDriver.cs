using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using MnLab.Approval.Models;
using Orchard.Localization;

namespace MnLab.Approval.Drivers {
    public class ApprovalPartDriver : ContentPartDriver<ApprovalPart> {

        private const string TemplateName = "Parts.Title.ApprovalPart";

        public Localizer T { get; set; }

        protected override string Prefix {
            get { return "Title"; }
        }

        protected override DriverResult Display(ApprovalPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Title",
                    () => shapeHelper.Parts_Title(Title: part.UserCommit)),
                ContentShape("Parts_Title_Summary",
                    () => shapeHelper.Parts_Title_Summary(Title: part.UserCommit)),
                ContentShape("Parts_Title_SummaryAdmin",
                    () => shapeHelper.Parts_Title_SummaryAdmin(Title: part.UserCommit))
                );
        }

        protected override DriverResult Editor(ApprovalPart part, dynamic shapeHelper) {

            return ContentShape("Parts_Title_Edit",
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

            context.ImportAttribute(part.PartDefinition.Name, "Title", title =>
                part.UserCommit = title
            );
        }

        protected override void Exporting(ApprovalPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.UserCommit);
        }
    }
}