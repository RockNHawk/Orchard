using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MnLab.PdfVisualDesign.Binding.Drivers;
using MnLab.PdfVisualDesign.Drivers;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace MnLab.PdfVisualDesign.Fields {


    public class TemplateSupportPartViewModel {

        public TempalteSupportPart Field;
        public IContent Content;
        public ContentItem ContentItem;
    }


    public class TempalteSupportPartDriver : ContentPartDriver<TempalteSupportPart> {
        public Localizer T { get; set; }
        private IOrchardServices _services { get; set; }
        private readonly IExtensionManager _extensionManager;
        IWorkContextAccessor _workContextAccessor;

        private const string TemplateName = "Parts_TempalteSupport";

        public TempalteSupportPartDriver(IExtensionManager extensionManager, IWorkContextAccessor workContextAccessor, IOrchardServices services) {
            T = NullLocalizer.Instance;
            _services = services;
            this._extensionManager = extensionManager;
            this._workContextAccessor = workContextAccessor;
        }



        protected override DriverResult Display(TempalteSupportPart part, string displayType, dynamic shapeHelper) {
            if (displayType == "Detail") {
                var workContext = _workContextAccessor.GetContext();
                workContext.CurrentTheme = _extensionManager.GetExtension("NHVSD");
            }
            return ContentShape(TemplateName, () => shapeHelper.Parts_TempalteSupport(Model: new TemplateSupportPartViewModel {
                Field = part,
                ContentItem = part.ContentItem,
            }));
        }

        protected override DriverResult Editor(TempalteSupportPart part, dynamic shapeHelper) {
            return ContentShape($"{TemplateName}",
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(TempalteSupportPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        //protected override void Importing(TempalteSupportPart part, ImportContentContext context) {
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null) {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "TempalteSupport", TempalteSupport =>
        //        part.UserCommit = TempalteSupport
        //    );
        //}

        //protected override void Exporting(TempalteSupportPart part, ExportContentContext context) {
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("TempalteSupport", part.UserCommit);
        //}

    }
}