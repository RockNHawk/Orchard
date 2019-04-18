using System;
using System.ComponentModel.DataAnnotations;
using MnLab.Enterprise;
using Orchard.ContentManagement.Records;

namespace MnLab.Enterprise.Approval {
    public class ApprovalSupportPartRecord : ContentPartRecord, IApprovalInfo {
        [StringLength(1024)]
        public virtual string CommitOpinion { get; set; }
        public virtual string AuditOpinion { get; set; }
        public virtual ApprovalStatus Status { get; set; }
        public virtual Type ApprovalType { get; set; }
        public virtual ApprovalPartRecord Latest { get; set; }
    }
}
