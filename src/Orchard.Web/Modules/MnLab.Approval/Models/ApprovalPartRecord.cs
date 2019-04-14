using System;
using System.ComponentModel.DataAnnotations;
using Bitlab.Enterprise;
using Orchard.ContentManagement.Records;

namespace MnLab.Approval.Models {
    public class ApprovalSupportPartRecord : ContentPartVersionRecord {

        [StringLength(1024)]
        public virtual string UserCommit { get; set; }

        public virtual string ApprovalComments { get; set; }
        public virtual ApprovalStatus ApprovalStatus { get; set; }
        public virtual Type ApprovalType { get; set; }
        public virtual IApproval CurrentApproval { get; set; }
    }
}
