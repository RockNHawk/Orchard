using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Pvlt.Localization {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("CulturePickerWidgetPart", builder => builder
                .WithDescription("A widget to switch culture and redirect to translated page.")
                .Attachable(false)
            );

            ContentDefinitionManager.AlterTypeDefinition("CulturePickerWidget", builder => builder
                .DisplayedAs("Culture Picker Widget")
                .WithPart("CulturePickerWidgetPart")
                .AsWidgetWithIdentity()
            );

            return 1;
        }
    }
}
