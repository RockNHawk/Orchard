using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace I9AOrchard.MediaPickerField.Settings
{
    public class MediaPickerFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "MediaPickerField")
            {
                var model = definition.Settings.GetModel<MediaPickerFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            var model = new MediaPickerFieldSettings();
            if (updateModel.TryUpdateModel(model, "MediaPickerFieldSettings", null, null))
            {
                builder.WithSetting("MediaPickerFieldSettings.MediaPathDefault", model.MediaPathDefault.ToString());
            }

            yield return DefinitionTemplate(model);
        }

    }
}