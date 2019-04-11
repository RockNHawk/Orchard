using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MnLab.PdfVisualDesign.Binding.Drivers;
using MnLab.PdfVisualDesign.Drivers;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace MnLab.PdfVisualDesign.Fields
{


    public class TemplateSupportViewModel {

        public TempalteSupportField Field;
        public IContent Content;
        public ContentItem ContentItem;
    }


    public class TempalteSupportFieldDriver : FieldDriverBase<TempalteSupportField> 
    {
        public Localizer T { get; set; }
        private IOrchardServices _services { get; set; }

        public TempalteSupportFieldDriver(IOrchardServices services)
        {
            T = NullLocalizer.Instance;
            _services = services;
        }

        protected override DriverResult Display(ContentPart part, TempalteSupportField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_TempalteSupportField", GetDifferentiator(field, part),
                () =>
                {
                    return shapeHelper.Fields_TempalteSupportField(Name: field.Name, Field: field);
                });
        }

        protected override DriverResult Editor(ContentPart part, TempalteSupportField field, dynamic shapeHelper)
        {
            
            return ContentShape("Fields_TempalteSupportField_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Fields_TempalteSupportField_Edit", 
                    Model: new TemplateSupportViewModel {
                        Field =field,
                        ContentItem= part.ContentItem,
                    }, 
                    Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, TempalteSupportField field, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null))
            {
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, TempalteSupportField field, ImportContentContext context)
        {
            var imageElement = context.Data.Element(field.FieldDefinition.Name + "." + field.Name);
            if (imageElement == null)
            {
                return;
            }

            var dataElement = imageElement.Element("HTML");
            if (dataElement != null)
            {
                field.HTML = dataElement.Value;
            }
        }

        protected override void Exporting(ContentPart part, TempalteSupportField field, ExportContentContext context)
        {
            var imageElement = context.Element(field.FieldDefinition.Name + "." + field.Name);
            imageElement.Add(new XElement("HTML", new XCData(field.HTML)));
        }

        private static Regex RemoveTagsRegex = new Regex(@"<[^>]*>|&[a-zA-Z]+;|&#[0-9]+;|<!--(.*?)-->", RegexOptions.Compiled | RegexOptions.Singleline);

        protected override void Describe(DescribeMembersContext context)
        {
            context
                .Member(null, typeof(string), T("HTML"), T("The HTML value of the field."))
                .Enumerate<TempalteSupportField>(() => field => new[] { RemoveTagsRegex.Replace(field.HTML, " ") });
        }
    }
}