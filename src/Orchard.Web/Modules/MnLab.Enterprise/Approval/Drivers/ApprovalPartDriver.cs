using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using Orchard.Localization;
using MnLab.Enterprise.Approval.Models;
using Drahcro.Data;
using Drahcro;
using Orchard;
using System.Collections.Generic;

namespace MnLab.Enterprise.Approval.Drivers {

    public class ApprovalViewModel {
        public ApprovalPart ApprovalPart { get; set; }
        public dynamic ContentEditor { get; set; }


        /// <summary>
        /// 审批员的审批意见
        /// </summary>
        public string AuditOpinion { get; set; }


    }

    public class ApprovalPartDriver : ContentPartDriver<ApprovalPart> {

        private const string TemplateName = "Parts.ApprovalPart";

        IContentManager _contentManager;
        IWorkContextAccessor workContextAccessor;
        //IContentPartRepository<ApprovalPart, ApprovalPartRecord> contentPartRepository;
        ContentPartRecordRepository contentPartRepository;
        public ApprovalPartDriver(
               IWorkContextAccessor workContextAccessor,
            IContentManager contentManager,
            ContentPartRecordRepository contentPartRepository
            ) {
            this._contentManager = contentManager;
            this.contentPartRepository = contentPartRepository;
            this.workContextAccessor = workContextAccessor;
            // T = NullLocalizer.Instance;
            // Logger = NullLogger.Instance;
        }

        //public Localizer T { get; set; }

        //protected override string Prefix {
        //    get { return "Approval"; }
        //}

        protected override DriverResult Display(ApprovalPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_ApprovalPart",
                    () => shapeHelper.Parts_ApprovalPart(Approval: part.CommitOpinion)),
                ContentShape("Parts_ApprovalPart_Summary",
                    () => shapeHelper.Parts_ApprovalPart_Summary(Approval: part.CommitOpinion)),
                ContentShape("Parts_ApprovalPart_SummaryAdmin",
                    () => shapeHelper.Parts_ApprovalPart_SummaryAdmin(Approval: part.CommitOpinion))
                );
        }


        //public static System.Threading.ThreadLocal<bool> IsCurrentInApproval = new System.Threading.ThreadLocal<bool>();

        protected override DriverResult Editor(ApprovalPart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ApprovalPart part, IUpdateModel updater, dynamic shapeHelper) {

            //contentPartRepository.Fill(part);

            //var referenceContentRecord = part.ContentRecord;
            //if (referenceContentRecord != null) {
            //    var content = _contentManager.Get(referenceContentRecord.Id);
            //    _contentManager.UpdateEditor(content, updater);
            //    //updater.TryUpdateModel(part, Prefix, null, null);
            //}

            //return Editor(part, shapeHelper);

            //IsCurrentInApproval.Value = true;
            workContextAccessor.GetContext().SetState("IsCurrentInApproval", 1);


            //System.Threading.Thread.CurrentThread
            //System.AppDomain.CurrentDomain.SetData("_isInApprovalEditor",1);
            //part.ContentItem.ContentManager.

            contentPartRepository.Fill(part);

            var vm = new ApprovalViewModel {
                ApprovalPart = part,
                // ContentEditor = contentEditor,
            };

            var referenceContentRecord = part.ContentRecord;
            if (referenceContentRecord != null) {
                var content = _contentManager.Get(referenceContentRecord.Id, VersionOptions.Latest);
                var contentEditor = _contentManager.BuildEditor(content);
                vm.ContentEditor = contentEditor;

                if (updater != null) {
                    //   if (updater.TryUpdateModel(part.Record, PrefixUtility.GetPrefix(Prefix, nameof(ApprovalViewModel.ApprovalPart)), new[] { nameof(ApprovalPartRecord.AuditOpinion) }, null)) {
                    if (updater.TryUpdateModel(part.Record, PrefixUtility.GetPrefix(Prefix, nameof(ApprovalViewModel.ApprovalPart)), new[] { nameof(ApprovalPartRecord.AuditOpinion) }, null)) {
                        if (part.Record.AuditOpinion != null) {
                            contentPartRepository.Update(part);
                        }
                    }

                    _contentManager.UpdateEditor(content, updater);
                    //updater.TryUpdateModel(part, Prefix, null, null);
                }
            }

            var results = new List<DriverResult> { };

            if (part.Status == ApprovalStatus.WaitingApproval) {
                results.Add(ContentShape("Content_ApproveButton", publishButton => publishButton));
                results.Add(ContentShape("Content_RejectButton", publishButton => publishButton));
            }

            results.Add(ContentShape("Parts_ApprovalPart_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: vm, Prefix: Prefix)));

            return Combined(results.ToArray());
        }

        //protected override void Importing(ApprovalPart part, ImportContentContext context) {
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null) {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "Approval", Approval =>
        //        part.CommitOpinion = Approval
        //    );
        //}

        //protected override void Exporting(ApprovalPart part, ExportContentContext context) {
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Approval", part.CommitOpinion);
        //}
    }
}