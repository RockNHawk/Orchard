﻿using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using Orchard.Localization;

namespace MnLab.Enterprise.Approval.Drivers {
    public class ApprovalSupportPartDriver : ContentPartDriver<ApprovalSupportPart> {

        //private const string TemplateName = "Parts.ApprovalSupport";

        public Localizer T { get; set; }

        //protected override string Prefix {
        //    get { return "ApprovalSupport"; }
        //}

        protected override DriverResult Display(ApprovalSupportPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_ApprovalSupport",
                    () => shapeHelper.Parts_ApprovalSupport(Approval: part.CommitOpinion)),
                ContentShape("Parts_ApprovalSupport_Summary",
                    () => shapeHelper.Parts_ApprovalSupport_Summary(Approval: part.CommitOpinion)),
                ContentShape("Parts_ApprovalSupport_SummaryAdmin",
                    () => shapeHelper.Parts_ApprovalSupport_SummaryAdmin(Approval: part.CommitOpinion))
                );
        }

        protected override DriverResult Editor(ApprovalSupportPart part, dynamic shapeHelper) {

            //return ContentShape("Parts_ApprovalSupport_Edit",
            //    () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));

            return Combined(
                ContentShape("Content_CommitButton", publishButton => publishButton),
                ContentShape("Parts_ApprovalSupport_Edit_Head",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.ApprovalSupport.Edit.Head", Model: part, Prefix: Prefix)),
                ContentShape("Parts_ApprovalSupport_Edit_Foot",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.ApprovalSupport.Edit.Foot", Model: part, Prefix: Prefix))
                );
        }

        protected override DriverResult Editor(ApprovalSupportPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        //protected override void Importing(ApprovalSupportPart part, ImportContentContext context) {
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null) {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "Approval", Approval =>
        //        part.CommitOpinion = Approval
        //    );
        //}

        //protected override void Exporting(ApprovalSupportPart part, ExportContentContext context) {
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Approval", part.CommitOpinion);
        //}
    }
}