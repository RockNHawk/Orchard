using Orchard;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.UI.Notify;
using Hazdesign.NewItem.Models;
namespace Hazdesign.NewItem.Drivers {
    public class NewItemSettingsPartDriver : ContentPartDriver<NewItemSettingsPart> {
        public NewItemSettingsPartDriver(
           INotifier notifier) {
           _notifier = notifier;
           T = NullLocalizer.Instance;
       }

       public Localizer T { get; set; }
       private const string TemplateName = "Parts/NewItem.Settings";
       private readonly INotifier _notifier;

       protected override DriverResult Editor(NewItemSettingsPart part, dynamic shapeHelper) {
           return ContentShape("Parts_NewItem_Settings",
                   () => shapeHelper.EditorTemplate(
                       TemplateName: TemplateName,
                       Model: part,
                       Prefix: Prefix));
       }

       protected override DriverResult Editor(NewItemSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
           if (updater.TryUpdateModel(part, Prefix, null, null)) {
               _notifier.Information(T("New Item settings updated successfully"));
           } else {
               _notifier.Error(T("Error during content New Item settings update!"));
           }
           return Editor(part, shapeHelper);
       }
    }
}