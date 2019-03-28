using System;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using I9AOrchard.MediaPickerField.ViewModels;
using I9AOrchard.MediaPickerField.Settings;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Environment.Extensions;

namespace I9AOrchard.MediaPickerField.Drivers
{
    [UsedImplicitly]
    public class MediaPickerFieldDriver : ContentFieldDriver<Fields.MediaPickerField>
    {
        public IOrchardServices Services { get; set; }

        // EditorTemplates/Fields/MediaPickerField.cshtml
        private const string TemplateName = "Fields/MediaPickerField";

        public MediaPickerFieldDriver(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            // handles spaces in field names
            return (part.PartDefinition.Name + "." + field.Name).Replace(" ", "_");
        }

        protected override DriverResult Display(ContentPart part, Fields.MediaPickerField field, string displayType, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<MediaPickerFieldSettings>();

            var viewModel = new MediaPickerFieldViewModel
            {
                Name = field.Name,
                MediaPickerField = field,
                PickerPath = field.PickerPath,
                EditorMediaPath = settings.MediaPathDefault
            };

            return ContentShape("Fields_MediaPickerField", // this is just a key in the Shape Table
                () =>
                    shapeHelper.Fields_MediaPickerField( // this is the actual Shape which will be resolved (Fields/MediaPickerField.cshtml)
                        ContentField: field,
                        Model: viewModel)
                    );
        }

        protected override DriverResult Editor(ContentPart part, Fields.MediaPickerField field, dynamic shapeHelper)
        {

            var settings = field.PartFieldDefinition.Settings.GetModel<MediaPickerFieldSettings>();

            var viewModel = new MediaPickerFieldViewModel
            {
                Name = field.Name,
                MediaPickerField = field,
                PickerPath = field.PickerPath,
                EditorMediaPath = settings.MediaPathDefault
            };

            return ContentShape("Fields_MediaPickerField_Edit", // this is just a key in the Shape Table
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, Fields.MediaPickerField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<MediaPickerFieldSettings>();

            var viewModel = new MediaPickerFieldViewModel
            {
                Name = field.Name,
                MediaPickerField = field,
                PickerPath = field.PickerPath,
                EditorMediaPath = settings.MediaPathDefault
            };

            updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null);


            return Editor(part, field, shapeHelper);
        }
    }
}