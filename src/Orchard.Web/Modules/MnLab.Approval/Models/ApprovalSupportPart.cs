using System;
using System.ComponentModel.DataAnnotations;
using Bitlab.Enterprise;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace MnLab.Enterprise.Approval.Models {
    public class ApprovalSupportPart : ContentPart<ApprovalSupportPartRecord>, IApprovalInfo {

        public string CommitOpinion { get { return Record.CommitOpinion; } set { Record.CommitOpinion = value; } }
        public string AuditOpinion { get { return Record.AuditOpinion; } set { Record.AuditOpinion = value; } }
        public ApprovalStatus Status { get { return Record.Status; } set { Record.Status = value; } }
        public Type ApprovalType { get { return Record.ApprovalType; } set { Record.ApprovalType = value; } }
        public IApproval CurrentApproval { get { return Record.CurrentApproval; } set { Record.CurrentApproval = value; } }


        //public virtual string ApprovalComments { get; set; }
        //public virtual ApprovalStatus ApprovalStatus { get; set; }
        //public virtual Type ApprovalType { get; set; }
        //public virtual IApproval CurrentApproval { get; set; }

    }
}