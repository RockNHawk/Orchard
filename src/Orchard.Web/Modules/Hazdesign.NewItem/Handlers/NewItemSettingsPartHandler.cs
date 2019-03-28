using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Hazdesign.NewItem.Models;
namespace Hazdesign.NewItem.Handlers {
    public class NewItemSettingsPartHandler : ContentHandler {
        public NewItemSettingsPartHandler(IRepository<NewItemSettingsPartRecord> repository) {
            Filters.Add(new ActivatingFilter<NewItemSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
       }
    }
}