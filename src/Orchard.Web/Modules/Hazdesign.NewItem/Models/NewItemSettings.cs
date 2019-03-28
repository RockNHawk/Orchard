using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
namespace Hazdesign.NewItem.Models {
    public class NewItemSettingsPart : ContentPart<NewItemSettingsPartRecord> {
        public int Days {
            get { return Record.Days; }
            set { Record.Days = value; }
        }
    }
    public class NewItemSettingsPartRecord : ContentPartRecord { 
        public virtual int Days { get; set; }
    }
}