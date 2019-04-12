using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace MnLab.PdfVisualDesign {
    public class TempalteSupportPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string Title { get; set; }
    }
}
