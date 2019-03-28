
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Guang.HiddenField.Fields;
using Orchard.Localization;

namespace Guang.HiddenField.Drivers
{
    
     public class GuangHiddenFieldDriver : ContentFieldDriver<GuangHiddenField>
    {
         public GuangHiddenFieldDriver()
        {           T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        private static string GetPrefix(GuangHiddenField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private string GetDifferentiator(GuangHiddenField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, GuangHiddenField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_GuangHiddenField_Text", GetDifferentiator(field, part),
                () => shapeHelper.Fields_GuangHiddenField_Text(ContentPart: part, ContentField: field, Name: field.Name, Value: field.Value));
        }

        protected override DriverResult Editor(ContentPart part, GuangHiddenField field, dynamic shapeHelper)
        {
            return ContentShape("Fields_GuangHiddenField_Text_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: "Fields.GuangHiddenField.Text.Edit", Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, GuangHiddenField field, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(field, GetPrefix(field, part), null, null);
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, GuangHiddenField field, ImportContentContext context)
        {
            var importedText = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "Text");
            if (importedText != null)
            {
                field.Value = importedText;
            }
        }

        protected override void Exporting(ContentPart part, GuangHiddenField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Text", field.Value);
        }
    }
}



