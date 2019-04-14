using System;
using System.ComponentModel.DataAnnotations;
using Bitlab.Enterprise;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

namespace MnLab.Approval.Models {
    public class ApprovalSupportPart : ContentPart<ApprovalSupportPartRecord> {

        //[Required]

        public string UserCommit {
            get { return Retrieve(x => x.UserCommit); }
            set { Store(x => x.UserCommit, value); }
        }

        public virtual string ApprovalComments { get; set; }
        public virtual ApprovalStatus ApprovalStatus { get; set; }
        public virtual Type ApprovalType { get; set; }
        public virtual IApproval CurrentApproval { get; set; }

    }
}