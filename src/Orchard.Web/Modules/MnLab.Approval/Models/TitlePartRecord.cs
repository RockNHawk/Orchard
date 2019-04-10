using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace MnLab.Approval.Models {
    public class ApprovalPartRecord : ContentPartVersionRecord {
        [StringLength(1024)]
        public virtual string Title { get; set; }
    }
}
