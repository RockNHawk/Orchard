using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace MnLab.Approval.Models {
    public class ApprovalSupportPartRecord : ContentPartVersionRecord {

        [StringLength(1024)]
        public virtual string UserCommit { get; set; }




    }
}
